using MediatR;

namespace ApplicationLayer.Features.Education.Query;

public record GetCourseCategoriesQuery() : IRequest<HandlerResult>;
