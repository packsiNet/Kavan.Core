using Microsoft.AspNetCore.Http;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Commands;

public record UploadLogoCommand(IFormFile File) : IRequest<HandlerResult>;