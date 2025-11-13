using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class DeleteCourseHandler(ICourseService _service) : IRequestHandler<DeleteCourseCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(request.Id);
        return result.ToHandlerResult();
    }
}

