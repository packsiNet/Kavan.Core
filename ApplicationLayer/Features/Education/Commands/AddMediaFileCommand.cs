using ApplicationLayer.DTOs.Education;
using MediatR;

namespace ApplicationLayer.Features.Education.Commands;

public record AddMediaFileCommand(AddMediaFileDto Model) : IRequest<HandlerResult>;