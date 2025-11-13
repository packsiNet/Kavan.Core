using ApplicationLayer.DTOs.Education;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Query;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class GetLessonStreamTokenHandler(
    IStreamAccessService streamAccessService,
    ApplicationLayer.Interfaces.IUserContextService userContext
) : IRequestHandler<GetLessonStreamTokenQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetLessonStreamTokenQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.UserId;
        if (userId is null)
            return Result<StreamTokenDto>.Failure(Error.Authentication()).ToHandlerResult();

        var result = await streamAccessService.IssueTokenAsync(request.CourseId, request.LessonId, userId.Value);
        return result.ToHandlerResult();
    }
}
