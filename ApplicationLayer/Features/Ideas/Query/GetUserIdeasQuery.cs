using ApplicationLayer.DTOs.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Query;

public record GetUserIdeasQuery(int UserId, GetIdeasRequestDto Model) : IRequest<HandlerResult>;