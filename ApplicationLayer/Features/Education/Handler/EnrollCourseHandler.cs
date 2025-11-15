using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class EnrollCourseHandler(ICourseEnrollmentService _service) : IRequestHandler<EnrollCourseCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.EnrollAsync(request.Model);
        return result.ToHandlerResult();
    }
}