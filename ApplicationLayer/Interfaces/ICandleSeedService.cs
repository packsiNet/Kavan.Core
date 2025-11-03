using System.Threading;

namespace ApplicationLayer.Interfaces;

public interface ICandleSeedService
{
    Task<int> SeedBTCUSDT_1m_DoubleTopBreakoutAsync(int count = 100, DateTime? startTime = null, CancellationToken cancellationToken = default);
    Task<int> SeedETHUSDT_MTF_FVG_StructureAsync(int days = 12, int m5Bars = 60, int m1Bars = 120, DateTime? startTime = null, CancellationToken cancellationToken = default);
}