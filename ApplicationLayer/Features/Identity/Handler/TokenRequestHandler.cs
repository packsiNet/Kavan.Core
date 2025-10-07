using ApplicationLayer.Common;
using ApplicationLayer.Common.Enums;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.DTOs.User;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.Utilities;
using ApplicationLayer.Features.Identity.Query;
using ApplicationLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.Features.Identity.Handler;

public class TokenRequestHandler(IIdentityService _identityService,
                                      IRefreshTokenService _refreshTokenService,
                                      IUserAccountServices _userAccountServices,
                                      IUnitOfWork _unitOfWork,
                                      ILogger<TokenRequestDto> _logger) : IRequestHandler<TokenRequestQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(TokenRequestQuery request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var result = await _refreshTokenService.VerifyTokenAsync(request.AccessTokens, request.RefreshToken);

            if (result.IsFailure)
            {
                await _unitOfWork.RollbackAsync();
                return result.ToHandlerResult();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var storedToken = result.Value;

            UserAccountKeyDto userAccountKeyDto = new() { Id = storedToken.UserAccountId };
            var userModel = await _userAccountServices.GetUserAccountByIdAsync(userAccountKeyDto);

            var token = await _identityService.TokenRequestGeneratorAsync(userModel.UserName, userModel.Id);

            var refreshToken = _refreshTokenService.RefreshTokenGenerator(userModel.Id, token.tokenId);
            refreshToken.UserFullName = storedToken.UserFullName;

            var refreshTokenResult = _refreshTokenService.AddRefreshToken(refreshToken);

            if (refreshTokenResult.IsFailure)
            {
                await _unitOfWork.RollbackAsync();
                return refreshTokenResult.ToHandlerResult();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();

            var authorizeResultViewModel = new AuthorizeResultDto()
            {
                AccessTokens = token.jwtToken,
                RefreshToken = refreshToken.Token,
                UserFullName = storedToken.UserFullName
            };

            return new HandlerResult
            {
                RequestStatus = RequestStatus.Successful,
                ObjectResult = authorizeResultViewModel,
                Message = CommonMessages.Successful
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(message: ex.Message, CommonMessages.Failed);
            return new HandlerResult
            {
                RequestStatus = RequestStatus.Failed,
                ObjectResult = request,
                Message = CommonMessages.Failed
            };
        }
    }
}