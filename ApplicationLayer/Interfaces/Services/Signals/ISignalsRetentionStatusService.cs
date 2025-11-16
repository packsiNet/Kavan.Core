using ApplicationLayer.Dto.SignalsRetention;

namespace ApplicationLayer.Interfaces.Services.Signals
{
    public interface ISignalsRetentionStatusService
    {
        RetentionStatusDto GetStatus();
        void Update(DateTime runUtc, int d1m, int d5m, int d1h, int d4h, int d1d, string lastError = "");
    }
}