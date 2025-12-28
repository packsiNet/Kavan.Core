using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.Trade;
using DomainLayer.Common.Enums;

namespace ApplicationLayer.Interfaces.Services;

public interface ITradeService
{
    Task<Result<TradeDto>> CreateTradeAsync(CreateTradeDto dto);
    Task<Result<List<TradeDto>>> GetTradesByPeriodAsync(int periodId);
    Task<Result<TradeDto>> CloseTradeAsync(int tradeId); // Manual Close
    Task<Result<TradeDto>> CloseTradeInternalAsync(int tradeId, decimal exitPrice, ExitReason reason); // For System/Service
    Task<Result<TradeDto>> UpdateTradeAsync(UpdateTradeDto dto);
    Task<Result<bool>> CancelTradeAsync(int tradeId, string reason);
    Task<Result<List<TradeCalendarDto>>> GetTradeCalendarAsync(int? periodId);
    Task CheckOpenTradesAsync(); // For background job
}
