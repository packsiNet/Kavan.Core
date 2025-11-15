using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class DeleteLessonHandler(ILessonService _service) : IRequestHandler<DeleteLessonCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(request.LessonId);
        return result.ToHandlerResult();
    }
}