using ApplicationLayer.Features.Plans.Query;
using ApplicationLayer.Features.Plans.Handler;
using ApplicationLayer.Interfaces.Services;
using ApplicationLayer.Dto.BaseDtos;
using DomainLayer.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Plans.Handlers;

public class GetPlansHandlerTests
{
    private class InMemoryPlanService : IPlanService
    {
        private readonly List<Plan> _plans;
        private readonly IMapper _mapper;

        public InMemoryPlanService(IMapper mapper)
        {
            _mapper = mapper;
            _plans = new List<Plan>
            {
                new Plan{ Id = 1, Code = "basic", Name = "Basic" },
                new Plan{ Id = 2, Code = "pro", Name = "Pro" }
            };
        }

        public Task<Result<ApplicationLayer.DTOs.Plans.PlanDto>> CreateAsync(ApplicationLayer.DTOs.Plans.CreatePlanDto dto)
            => Task.FromResult(Result<ApplicationLayer.DTOs.Plans.PlanDto>.Failure(RequestStatus.BadRequest, "Not implemented in test"));

        public Task<Result<ApplicationLayer.DTOs.Plans.PlanDto>> UpdateAsync(int id, ApplicationLayer.DTOs.Plans.UpdatePlanDto dto)
            => Task.FromResult(Result<ApplicationLayer.DTOs.Plans.PlanDto>.Failure(RequestStatus.BadRequest, "Not implemented in test"));

        public Task<Result> DeleteAsync(int id)
            => Task.FromResult(Result.Failure(RequestStatus.BadRequest, "Not implemented in test"));

        public Task<Result<ApplicationLayer.DTOs.Plans.PlanDto>> GetByIdAsync(int id)
        {
            var p = _plans.FirstOrDefault(x => x.Id == id);
            if (p == null) return Task.FromResult(Result<ApplicationLayer.DTOs.Plans.PlanDto>.Failure(RequestStatus.NotFound, "Not found"));
            return Task.FromResult(Result<ApplicationLayer.DTOs.Plans.PlanDto>.Success(_mapper.Map<ApplicationLayer.DTOs.Plans.PlanDto>(p)));
        }

        public Task<Result<List<ApplicationLayer.DTOs.Plans.PlanDto>>> GetAllAsync()
        {
            var list = _mapper.Map<List<ApplicationLayer.DTOs.Plans.PlanDto>>(_plans);
            return Task.FromResult(Result<List<ApplicationLayer.DTOs.Plans.PlanDto>>.Success(list));
        }
    }

    [Fact]
    public async Task GetPlansHandler_ReturnsListOfPlans()
    {
        var mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ApplicationLayer.Mapping.Plans.PlanProfile());
        }).CreateMapper();

        IPlanService service = new InMemoryPlanService(mapper);
        var handler = new GetPlansHandler(service);
        var result = await handler.Handle(new GetPlansQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.Count >= 2);
        Assert.Contains(result.Value, p => p.Code == "basic");
        Assert.Contains(result.Value, p => p.Code == "pro");
    }
}