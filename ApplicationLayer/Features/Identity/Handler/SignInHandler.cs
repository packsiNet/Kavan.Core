using ApplicationLayer.Common.Enums;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.Utilities;
using ApplicationLayer.Features.Identity.Commands;
using ApplicationLayer.Interfaces;
using MediatR;

namespace ApplicationLayer.Features.Identity.Handler;

public class SignInHandler(IUserAccountServices _userAccountServices,
                           IIdentityService _identityService,
                           IRefreshTokenService _refreshTokenService,
                           IUnitOfWork _uow) : IRequestHandler<SignInCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Model;

        var userResult = _userAccountServices.GetUserByValidationMethodAsync(dto);
        if (!userResult.RequestStatus.Equals(RequestStatus.Successful))
            return userResult.ToResult().ToHandlerResult();

        var user = (DomainLayer.Entities.UserAccount)userResult.Data;

        await _uow.BeginTransactionAsync();
        AuthorizeResultDto authDto;

        if (dto.ValidationMethod == ValidationMethodEnum.UserInformation.Value)
        {
            var auth = await _identityService.AuthenticateUserInformationAsync(dto, user);
            if (!auth.RequestStatus.Equals(RequestStatus.Successful))
            {
                await _uow.RollbackAsync();
                return auth.ToResult().ToHandlerResult();
            }

            var ar = (AuthorizeResultDto)auth.Data;
            var refresh = _refreshTokenService.RefreshTokenGenerator(user.Id, ar.TokenId);
            refresh.UserFullName = user.UserName;
            var addRefresh = _refreshTokenService.AddRefreshToken(refresh);
            if (addRefresh.IsFailure)
            {
                await _uow.RollbackAsync();
                return addRefresh.ToHandlerResult();
            }

            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitAsync();

            authDto = new AuthorizeResultDto
            {
                AccessTokens = ar.AccessTokens,
                RefreshToken = refresh.Token,
                TokenId = ar.TokenId,
                UserFullName = user.UserName
            };
        }
        else
        {
            var otp = _identityService.AuthenticateOneTimePassword(dto, user);
            if (!otp.RequestStatus.Equals(RequestStatus.Successful))
            {
                await _uow.RollbackAsync();
                return otp.ToResult().ToHandlerResult();
            }

            var token = await _identityService.TokenRequestGeneratorAsync(user.UserName, user.Id);
            var refresh = _refreshTokenService.RefreshTokenGenerator(user.Id, token.tokenId);
            refresh.UserFullName = user.UserName;
            var addRefresh = _refreshTokenService.AddRefreshToken(refresh);
            if (addRefresh.IsFailure)
            {
                await _uow.RollbackAsync();
                return addRefresh.ToHandlerResult();
            }

            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitAsync();

            authDto = new AuthorizeResultDto
            {
                AccessTokens = token.jwtToken,
                RefreshToken = refresh.Token,
                TokenId = token.tokenId,
                UserFullName = user.UserName
            };
        }

        return new HandlerResult
        {
            RequestStatus = ApplicationLayer.Common.Enums.RequestStatus.Successful,
            ObjectResult = authDto,
            Message = ApplicationLayer.Common.CommonMessages.Successful
        };
    }
}