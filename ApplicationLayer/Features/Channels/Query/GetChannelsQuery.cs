using ApplicationLayer.DTOs.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Query;

public record GetChannelsQuery(GetChannelsRequestDto Model) : IRequest<HandlerResult>;