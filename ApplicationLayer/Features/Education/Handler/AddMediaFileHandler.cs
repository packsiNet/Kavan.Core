using ApplicationLayer.Extensions;
using ApplicationLayer.Features.Education.Commands;
using ApplicationLayer.Interfaces.Services;
using MediatR;

namespace ApplicationLayer.Features.Education.Handler;

public class AddMediaFileHandler(ILessonService _service) : IRequestHandler<AddMediaFileCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(AddMediaFileCommand request, CancellationToken cancellationToken)
    {
        var result = await _service.AddMediaAsync(request.Model);
        return result.ToHandlerResult();
    }
}