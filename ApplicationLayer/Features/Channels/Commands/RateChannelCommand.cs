using ApplicationLayer.DTOs.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Commands;

public record RateChannelCommand(RateChannelDto Model) : IRequest<HandlerResult>;