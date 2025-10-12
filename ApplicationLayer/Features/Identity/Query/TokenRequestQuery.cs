using MediatR;

namespace ApplicationLayer.Features.Identity.Query;

public record TokenRequestQuery(string AccessTokens, string RefreshToken) : IRequest<HandlerResult>;