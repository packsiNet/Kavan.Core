using ApplicationLayer.DTOs.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Query;

public record GetIdeasQuery(GetIdeasRequestDto Model) : IRequest<HandlerResult>;