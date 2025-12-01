using ApplicationLayer.DTOs.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Query;

public record GetMyChannelsQuery(GetChannelsRequestDto Model) : IRequest<HandlerResult>;