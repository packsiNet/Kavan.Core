using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Portfolio;

namespace ApplicationLayer.Interfaces.Portfolio;

public interface IPortfolioService
{
    Task<Result<PortfolioEntryDto>> AddEntryAsync(CreatePortfolioEntryDto dto);
    Task<Result<PortfolioEntryDto>> UpdateEntryAsync(int id, UpdatePortfolioEntryDto dto);
    Task<Result> DeleteEntryAsync(int id);
    Task<Result> DeleteSymbolAsync(string symbol);
    Task<Result<PortfolioPositionsPageDto>> GetPositionsAsync(GetPortfolioRequestDto dto);
    Task<Result<PortfolioEntriesPageDto>> GetEntriesBySymbolAsync(string symbol, GetPortfolioEntriesRequestDto dto);
    Task<Result> AddSaleAsync(CreatePortfolioSaleDto dto);
}
