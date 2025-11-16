using ApplicationLayer.Dto.SignalsRetention;
using ApplicationLayer.Interfaces.Services.Signals;
using DomainLayer.Common.Attributes;

namespace InfrastructureLayer.BusinessLogic.Services.Signals;

[InjectAsSingleton]
public class SignalsRetentionStatusService : ISignalsRetentionStatusService
{
    private readonly object _lock = new();
    private RetentionStatusDto _status = new();

    public RetentionStatusDto GetStatus()
    {
        lock (_lock)
        {
            return new RetentionStatusDto
            {
                LastRunUtc = _status.LastRunUtc,
                Deleted_1m = _status.Deleted_1m,
                Deleted_5m = _status.Deleted_5m,
                Deleted_1h = _status.Deleted_1h,
                Deleted_4h = _status.Deleted_4h,
                Deleted_1d = _status.Deleted_1d,
                LastError = _status.LastError
            };
        }
    }

    public void Update(DateTime runUtc, int d1m, int d5m, int d1h, int d4h, int d1d, string lastError = "")
    {
        lock (_lock)
        {
            _status.LastRunUtc = runUtc;
            _status.Deleted_1m = d1m;
            _status.Deleted_5m = d5m;
            _status.Deleted_1h = d1h;
            _status.Deleted_4h = d4h;
            _status.Deleted_1d = d1d;
            _status.LastError = lastError ?? string.Empty;
        }
    }
}