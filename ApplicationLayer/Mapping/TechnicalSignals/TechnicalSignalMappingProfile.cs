using ApplicationLayer.Dto.TechnicalSignals;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.Mapping.TechnicalSignals;

public class TechnicalSignalMappingProfile : Profile
{
    public TechnicalSignalMappingProfile()
    {
        CreateMap<TechnicalSignal, TechnicalSignalDto>()
            .ForMember(dest => dest.SignalTypeText, opt => opt.Ignore()) // Will be set in handler
            .ForMember(dest => dest.DetailedSignalTypeText, opt => opt.Ignore()); // Will be set in handler
    }
}