using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.MarketAnalysis;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.MarketAnalysis.Commands;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services;
using AutoMapper;
using MediatR;
using System.Diagnostics;

namespace ApplicationLayer.Features.MarketAnalysis.Handler;

public class AnalyzeMarketHandler : IRequestHandler<AnalyzeMarketCommand, HandlerResult>
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

    public async Task<HandlerResult> Handle(AnalyzeMarketCommand request, CancellationToken cancellationToken)
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

            return Result<MarketAnalysisResponseDto>.Success(response).ToHandlerResult();
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            return Result<MarketAnalysisResponseDto>.GeneralFailure(ex.Message).ToHandlerResult();
        }
    }
}