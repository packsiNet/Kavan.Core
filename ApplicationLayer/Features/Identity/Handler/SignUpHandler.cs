using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Identity.Commands;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Extensions.Utilities;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.Identity.Handler;

public class SignUpHandler(IUserAccountServices _userAccountServices,
                           IIdentityService _identityService,
                           IRefreshTokenService _refreshTokenService,
                           IUnitOfWork _uow) : IRequestHandler<SignUpCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Model;

        string securityStamp;
        var hashed = HashGenerator.GenerateSHA256HashWithSalt(dto.Password, out securityStamp);

        var user = new UserAccount
        {
            UserName = dto.PhoneNumber,
            Password = hashed,
            SecurityStamp = securityStamp,
            Email = dto.Email,
            PhonePrefix = dto.PhonePrefix,
            PhoneNumber = dto.PhoneNumber,
            InviteCode = dto.InviteCode,
            ConfirmPhoneNumber = true
        };

        await _uow.BeginTransactionAsync();
        var addUser = await _userAccountServices.AddUserAccountAsync(user);
        if (addUser.IsFailure)
        {
            await _uow.RollbackAsync();
            return addUser.ToHandlerResult();
        }

        await _uow.SaveChangesAsync(cancellationToken);
        var newUserid = addUser.Value.Id;

        var profileResult = await _userAccountServices.AddProfileAsync(new UserProfile
        {
            UserAccountId = newUserid,
            DisplayName = dto.DisplayName
        });

        if (!profileResult.RequestStatus.Equals(Common.Enums.RequestStatus.Successful))
        {
            await _uow.RollbackAsync();
            return profileResult.ToResult().ToHandlerResult();
        }

        var roleAssign = await _userAccountServices.AssignRoleToUserAsync(new DTOs.User.UserAccountKeyDto { Id = newUserid }, "User");
        if (!roleAssign.RequestStatus.Equals(Common.Enums.RequestStatus.Successful))
        {
            await _uow.RollbackAsync();
            return roleAssign.ToResult().ToHandlerResult();
        }

        var token = await _identityService.TokenRequestGeneratorAsync(user.UserName, newUserid);
        var refresh = _refreshTokenService.RefreshTokenGenerator(newUserid, token.tokenId);
        refresh.UserFullName = dto.DisplayName;
        var addRefresh = _refreshTokenService.AddRefreshToken(refresh);
        if (addRefresh.IsFailure)
        {
            await _uow.RollbackAsync();
            return addRefresh.ToHandlerResult();
        }

        await _uow.SaveChangesAsync(cancellationToken);
        await _uow.CommitAsync();

        var auth = new AuthorizeResultDto
        {
            AccessTokens = token.jwtToken,
            RefreshToken = refresh.Token,
            TokenId = token.tokenId,
            UserFullName = dto.DisplayName
        };

        return new HandlerResult
        {
            RequestStatus = ApplicationLayer.Common.Enums.RequestStatus.Successful,
            ObjectResult = auth,
            Message = ApplicationLayer.Common.CommonMessages.Successful
        };
    }
}