using ApplicationLayer.Dto.MarketAnalysis;
using ApplicationLayer.Features.MarketAnalysis.Commands;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using AutoMapper;
using MediatR;
using System.Diagnostics;

namespace ApplicationLayer.Features.MarketAnalysis.Handler;

public class AnalyzeMarketHandler : IRequestHandler<AnalyzeMarketCommand, MarketAnalysisResponseDto>
{
    private readonly IMarketAnalysisService _marketAnalysisService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AnalyzeMarketHandler(
        IMarketAnalysisService marketAnalysisService,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _marketAnalysisService = marketAnalysisService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<MarketAnalysisResponseDto> Handle(AnalyzeMarketCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        try
        {
            // Perform market analysis
            var analysisResult = await _marketAnalysisService.AnalyzeMarketAsync(request.Request, cancellationToken);
            
            stopwatch.Stop();

            // Map to response DTO
            var response = _mapper.Map<MarketAnalysisResponseDto>(analysisResult);
            response.RequestId = requestId;
            response.ProcessingTimeMs = stopwatch.Elapsed.TotalMilliseconds;

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            return new MarketAnalysisResponseDto
            {
                RequestId = requestId,
                Success = false,
                ProcessingTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                Errors = new List<string> { ex.Message },
                Signals = new List<TradingSignalDto>(),
                Metadata = new AnalysisMetadataDto
                {
                    Errors = new List<string> { ex.Message }
                }
            };
        }
    }
}