using Microsoft.AspNetCore.Http;
using MediatR;

namespace ApplicationLayer.Features.Profiles.Organizations.Commands;

public record UploadBannerCommand(IFormFile File) : IRequest<HandlerResult>;