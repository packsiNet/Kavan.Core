using ApplicationLayer.DTOs.Education;
using MediatR;

namespace ApplicationLayer.Features.Education.Commands;

public record SetCoursePricingCommand(int Id, SetCoursePricingDto Model) : IRequest<HandlerResult>;

