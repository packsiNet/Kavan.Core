using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.FinancialPeriod;

namespace ApplicationLayer.Interfaces.Services;

public interface IFinancialPeriodService
{
    Task<Result<FinancialPeriodDto>> CreateAsync(CreateFinancialPeriodDto dto);
    Task<Result<FinancialPeriodDto>> GetActivePeriodAsync();
    Task<Result<FinancialPeriodDto>> ClosePeriodAsync(int id);
    // Add other methods as needed
}
