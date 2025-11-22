using ApplicationLayer.DTOs.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Commands;

public record CreateIdeaCommand(CreateIdeaDto Model) : IRequest<HandlerResult>;