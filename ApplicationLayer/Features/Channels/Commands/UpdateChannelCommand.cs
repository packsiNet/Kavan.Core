using ApplicationLayer.DTOs.Channels;
using MediatR;

namespace ApplicationLayer.Features.Channels.Commands;

public record UpdateChannelCommand(int Id, UpdateChannelDto Model) : IRequest<HandlerResult>;