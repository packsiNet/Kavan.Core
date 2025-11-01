using ApplicationLayer.Dto.TimeFrames;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.Mapping.TimeFrames;

public class TimeFrameMappingProfile : Profile
{
    public TimeFrameMappingProfile()
    {
        CreateMap<TimeFrame, TimeFrameDto>();
    }
}