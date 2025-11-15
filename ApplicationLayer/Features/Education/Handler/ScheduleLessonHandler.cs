using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class ScheduleLessonHandler(ILessonService _service) : IRequestHandler<ScheduleLessonCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(ScheduleLessonCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.ScheduleAsync(request.LessonId, request.Model.PublishAt);
        return result.ToHandlerResult();
    }
}