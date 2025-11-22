using ApplicationLayer.DTOs.Ideas;
using MediatR;

namespace ApplicationLayer.Features.Ideas.Commands;

public record UpdateIdeaCommand(int Id, UpdateIdeaDto Model) : IRequest<HandlerResult>;