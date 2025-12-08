using ApplicationLayer.Dto.BaseDtos;
using MediatR;

namespace ApplicationLayer.Features.Channels.Commands;

public record MuteChannelCommand(int ChannelId) : IRequest<HandlerResult>;
