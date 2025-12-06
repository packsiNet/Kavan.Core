using ApplicationLayer.DTOs.User;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Users.Commands;

public record UpdateMyUserProfileCommand(UpdateUserProfileDto Model) : IRequest<HandlerResult>;
