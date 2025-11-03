using System.Threading;

namespace ApplicationLayer.Interfaces;

public interface ICandleSeedService
{
    Task<int> SeedBTCUSDT_1m_DoubleTopBreakoutAsync(int count = 100, DateTime? startTime = null, CancellationToken cancellationToken = default);
}