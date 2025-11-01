using ApplicationLayer.Dto.MarketAnalysis;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.Mapping.MarketAnalysis;

public class MarketAnalysisProfile : Profile
{
    public MarketAnalysisProfile()
    {
        // MarketAnalysisResult to MarketAnalysisResponseDto
        CreateMap<MarketAnalysisResult, MarketAnalysisResponseDto>()
            .ForMember(dest => dest.ProcessingTimeMs, opt => opt.MapFrom(src => src.ProcessingTime.TotalMilliseconds))
            .ForMember(dest => dest.Success, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Errors, opt => opt.MapFrom(src => src.Metadata.Errors));

        // TradingSignal to TradingSignalDto
        CreateMap<TradingSignal, TradingSignalDto>();

        // SignalReason to SignalReasonDto
        CreateMap<SignalReason, SignalReasonDto>();

        // ConfirmedIndicator to ConfirmedIndicatorDto
        CreateMap<ConfirmedIndicator, ConfirmedIndicatorDto>();

        // SignalTargets to SignalTargetsDto
        CreateMap<SignalTargets, SignalTargetsDto>();

        // AnalysisMetadata to AnalysisMetadataDto
        CreateMap<AnalysisMetadata, AnalysisMetadataDto>();

        // Reverse mappings (if needed)
        CreateMap<MarketAnalysisRequestDto, MarketAnalysisRequest>();
        CreateMap<AnalysisConditionDto, AnalysisCondition>();
        CreateMap<ConditionConfirmationDto, ConditionConfirmation>();
        CreateMap<AnalysisFiltersDto, AnalysisFilters>();
        CreateMap<AnalysisPreferencesDto, AnalysisPreferences>();
    }
}