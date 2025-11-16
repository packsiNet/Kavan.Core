using ApplicationLayer.Dto.SignalsRetention;
using ApplicationLayer.Features.SignalsRetention.Query;
using ApplicationLayer.Interfaces.Services.Signals;
using MediatR;

namespace ApplicationLayer.Features.SignalsRetention.Handler
{
    public class GetRetentionStatusHandler : IRequestHandler<GetRetentionStatusQuery, RetentionStatusDto>
    {
        private readonly ISignalsRetentionStatusService _status;

        public GetRetentionStatusHandler(ISignalsRetentionStatusService status)
        {
            _status = status;
        }

        public Task<RetentionStatusDto> Handle(GetRetentionStatusQuery request, CancellationToken cancellationToken)
            => Task.FromResult(_status.GetStatus());
    }
}