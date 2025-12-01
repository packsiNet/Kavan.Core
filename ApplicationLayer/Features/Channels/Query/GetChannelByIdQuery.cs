using MediatR;

namespace ApplicationLayer.Features.Channels.Query;

public record GetChannelByIdQuery(int Id) : IRequest<HandlerResult>;