using ApplicationLayer.DTOs.Identity;
using MediatR;

namespace ApplicationLayer.Features.Identity.Commands;

public record SignInCommand(SignInDto Model) : IRequest<HandlerResult>;