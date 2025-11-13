using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class UpdateCourseHandler(ICourseService _service) : IRequestHandler<UpdateCourseCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(request.Id, request.Model);
        return result.ToHandlerResult();
    }
}

