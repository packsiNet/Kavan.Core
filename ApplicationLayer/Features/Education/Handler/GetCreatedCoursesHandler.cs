using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Query;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class GetCreatedCoursesHandler(IInstructorCourseQueryService service) : IRequestHandler<GetCreatedCoursesQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetCreatedCoursesQuery request, CancellationToken cancellationToken)
    {
        var result = await service.GetMyCreatedAsync();
        return result.ToHandlerResult();
    }
}
