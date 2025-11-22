using ApplicationLayer.DTOs.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Query;

public record GetMyIdeasQuery(GetIdeasRequestDto Model) : IRequest<HandlerResult>;