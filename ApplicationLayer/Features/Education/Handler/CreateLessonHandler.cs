using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class CreateLessonHandler(ILessonService _service) : IRequestHandler<CreateLessonCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request.Model);
        return result.ToHandlerResult();
    }
}