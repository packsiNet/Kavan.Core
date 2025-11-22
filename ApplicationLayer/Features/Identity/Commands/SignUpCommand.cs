using ApplicationLayer.DTOs.Identity;
using MediatR;

namespace ApplicationLayer.Features.Identity.Commands;

public record SignUpCommand(SignUpDto Model) : IRequest<HandlerResult>;