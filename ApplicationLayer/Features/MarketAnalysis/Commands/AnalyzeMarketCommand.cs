using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.MarketAnalysis;
using MediatR;

namespace ApplicationLayer.Features.MarketAnalysis.Commands;

public record AnalyzeMarketCommand(MarketAnalysisRequestDto Request) : IRequest<HandlerResult>;