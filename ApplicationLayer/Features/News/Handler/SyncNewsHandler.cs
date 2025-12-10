using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.News.Commands;
using ApplicationLayer.Interfaces.Services.News;
using MediatR;

namespace ApplicationLayer.Features.News.Handler;

public class SyncNewsHandler(INewsSyncService _service) : IRequestHandler<SyncNewsCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(SyncNewsCommand request, CancellationToken cancellationToken)
    {
        var count = await _service.SyncLatestAsync(request.Query, cancellationToken);
        return Result<int>.Success(count).ToHandlerResult();
    }
}
