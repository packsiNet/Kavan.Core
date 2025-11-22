using MediatR;

namespace ApplicationLayer.Features.Ideas.Query;

public record GetIdeaByIdQuery(int Id) : IRequest<HandlerResult>;