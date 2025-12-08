using ApplicationLayer.DTOs.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Query;

public record GetCreatedChannelsQuery(GetChannelsRequestDto Model) : IRequest<HandlerResult>;
