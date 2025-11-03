using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.RefreshTokens;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.Utilities;
using ApplicationLayer.Features.Identity.Commands;
using ApplicationLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.Features.Identity.Handler;

public class RevokeRefreshTokenHandler(IRefreshTokenService _refreshTokenService,
                                              IUnitOfWork _unitOfWork,
                                              ILogger<RevokeRefreshTokenDto> _logger) : IRequestHandler<RevokeRefreshTokenCommand, HandlerResult>
{

    public async Task<HandlerResult> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _refreshTokenService.RevokeRefreshTokenByUserIdAsync(request.UserId);

            if (result.IsFailure)
                return result.ToHandlerResult();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result.ToHandlerResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در لغو توکن‌های کاربر");
            return Result.GeneralFailure("خطا در لغو توکن‌های کاربر").ToHandlerResult();
        }
    }
}