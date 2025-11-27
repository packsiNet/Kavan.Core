using ApplicationLayer.DTOs.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Commands;

public record CreateChannelCommand(CreateChannelDto Model) : IRequest<HandlerResult>;