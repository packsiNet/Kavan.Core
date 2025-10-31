using ApplicationLayer.DTOs.Plans;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.Mapping.Plans;

public class PlanProfile : Profile
{
    public PlanProfile()
    {
        CreateMap<CreatePlanDto, Plan>();
        CreateMap<PlanFeatureDto, PlanFeature>();

        CreateMap<UpdatePlanDto, Plan>();

        CreateMap<Plan, PlanDto>()
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features));
        CreateMap<PlanFeature, PlanFeatureDto>();

        CreateMap<UserPlan, UserPlanDto>();
    }
}