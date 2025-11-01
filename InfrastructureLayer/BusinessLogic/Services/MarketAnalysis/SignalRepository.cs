using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.MarketAnalysis;

[InjectAsScoped]
public class SignalRepository : ISignalRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<MarketAnalysisResult> _marketAnalysisRepository;

    public SignalRepository(IUnitOfWork unitOfWork, IRepository<MarketAnalysisResult> marketAnalysisRepository)
    {
        _unitOfWork = unitOfWork;
        _marketAnalysisRepository = marketAnalysisRepository;
    }

    public async Task<List<TradingSignal>> GetActiveSignalsAsync(
        string symbol = null,
        string timeframe = null,
        string signalType = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        // Get all market analysis results and extract active signals
        var results = await _marketAnalysisRepository.Query().ToListAsync();

        var activeSignals = new List<TradingSignal>();

        foreach (var result in results)
        {
            var signals = result.Signals.Where(s => s.IsActive);

            // Apply filters
            if (!string.IsNullOrEmpty(symbol))
                signals = signals.Where(s => s.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(timeframe))
                signals = signals.Where(s => s.Timeframe.Equals(timeframe, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(signalType))
                signals = signals.Where(s => s.SignalType.Equals(signalType, StringComparison.OrdinalIgnoreCase));

            activeSignals.AddRange(signals);
        }

        // Apply pagination
        var orderedSignals = activeSignals.OrderByDescending(s => s.GeneratedAt);
        var skip = (pageNumber - 1) * pageSize;

        return orderedSignals.Skip(skip).Take(pageSize).ToList();
    }

    public async Task<List<TradingSignal>> GetSignalsBySymbolAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var results = await _marketAnalysisRepository.Query().ToListAsync();

        var symbolSignals = new List<TradingSignal>();

        foreach (var result in results)
        {
            var signals = result.Signals
                .Where(s => s.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
                .ToList();
            symbolSignals.AddRange(signals);
        }

        return symbolSignals.OrderByDescending(s => s.GeneratedAt).ToList();
    }

    public async Task<TradingSignal?> GetSignalByIdAsync(string signalId, CancellationToken cancellationToken = default)
    {
        // Since TradingSignal doesn't have an ID property, we'll search by a combination of properties
        // This is a simplified implementation - in a real scenario, you'd want to add an ID property
        var results = _marketAnalysisRepository.GetAll();

        foreach (var result in results)
        {
            var signal = result.Signals.FirstOrDefault(s =>
                $"{s.Symbol}_{s.Timeframe}_{s.GeneratedAt:yyyyMMddHHmmss}" == signalId);

            if (signal != null)
                return signal;
        }

        return null;
    }

    public async Task SaveSignalAsync(TradingSignal signal, CancellationToken cancellationToken = default)
    {
        // Create a new MarketAnalysisResult to store the signal
        var analysisResult = new MarketAnalysisResult
        {
            RequestId = Guid.NewGuid().ToString(),
            Signals = new List<TradingSignal> { signal },
            AnalyzedAt = DateTime.UtcNow,
            TotalSymbolsAnalyzed = 1,
            SignalsGenerated = 1,
            Metadata = new AnalysisMetadata()
        };

        await _marketAnalysisRepository.AddAsync(analysisResult);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateSignalAsync(TradingSignal signal, CancellationToken cancellationToken = default)
    {
        var results = await _marketAnalysisRepository.Query().ToListAsync();

        foreach (var result in results)
        {
            var existingSignal = result.Signals.FirstOrDefault(s =>
                s.Symbol == signal.Symbol &&
                s.Timeframe == signal.Timeframe &&
                s.GeneratedAt == signal.GeneratedAt);

            if (existingSignal != null)
            {
                // Update signal properties
                existingSignal.SignalType = signal.SignalType;
                existingSignal.Price = signal.Price;
                existingSignal.Confidence = signal.Confidence;
                existingSignal.Strength = signal.Strength;
                existingSignal.RiskLevel = signal.RiskLevel;
                existingSignal.IsActive = signal.IsActive;
                existingSignal.Reasons = signal.Reasons;
                existingSignal.ConfirmedIndicators = signal.ConfirmedIndicators;
                existingSignal.Targets = signal.Targets;

                await _marketAnalysisRepository.UpdateAsync(result);
                await _unitOfWork.SaveChangesAsync();
                break;
            }
        }
    }

    public async Task DeactivateSignalAsync(string signalId, CancellationToken cancellationToken = default)
    {
        var results = _marketAnalysisRepository.GetAll();

        foreach (var result in results)
        {
            var signal = result.Signals.FirstOrDefault(s =>
                $"{s.Symbol}_{s.Timeframe}_{s.GeneratedAt:yyyyMMddHHmmss}" == signalId);

            if (signal != null)
            {
                signal.IsActive = false;
                await _marketAnalysisRepository.UpdateAsync(result);
                await _unitOfWork.SaveChangesAsync();
                break;
            }
        }
    }

    public async Task<bool> SignalExistsAsync(string symbol, string signalType, string timeframe, CancellationToken cancellationToken = default)
    {
        var results = _marketAnalysisRepository.GetAll();

        foreach (var result in results)
        {
            var signalExists = result.Signals.Any(s =>
                s.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase) &&
                s.SignalType.Equals(signalType, StringComparison.OrdinalIgnoreCase) &&
                s.Timeframe.Equals(timeframe, StringComparison.OrdinalIgnoreCase) &&
                s.IsActive);

            if (signalExists)
                return true;
        }

        return false;
    }
}