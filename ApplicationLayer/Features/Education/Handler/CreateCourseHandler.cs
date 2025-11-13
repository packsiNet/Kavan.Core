using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class CreateCourseHandler(ICourseService _service) : IRequestHandler<CreateCourseCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request.Model);
        return result.ToHandlerResult();
    }
}

