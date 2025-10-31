using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Plans;
using ApplicationLayer.Interfaces.Services;
using AutoMapper;
using DomainLayer.Entities;
using InfrastructureLayer.BusinessLogic.Services.Plans;
using Tests.Unit.Plans.Fakes;

namespace Tests.Unit.Plans;

public class PlanServiceTests
{
    private readonly IMapper _mapper;

    public PlanServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ApplicationLayer.Mapping.Plans.PlanProfile());
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task CreateAsync_DuplicateCode_ReturnsDuplicateFailure()
    {
        var uow = new FakeUnitOfWork();
        var repoPlans = new Fakes.FakeRepository<Plan>();
        var repoFeatures = new Fakes.FakeRepository<PlanFeature>();

        // seed duplicate
        await repoPlans.AddAsync(new Plan { Code = "basic", Name = "Basic" });

        IPlanService service = new PlanService(uow, _mapper, repoPlans, repoFeatures);
        var result = await service.CreateAsync(new CreatePlanDto { Code = "basic", Name = "Another" });

        Assert.True(result.IsFailure);
        Assert.Equal(RequestStatus.Duplicated, result.RequestStatus);
    }

    [Fact]
    public async Task CreateAsync_Success_ReturnsSuccessWithDto()
    {
        var uow = new FakeUnitOfWork();
        var repoPlans = new Fakes.FakeRepository<Plan>();
        var repoFeatures = new Fakes.FakeRepository<PlanFeature>();

        IPlanService service = new PlanService(uow, _mapper, repoPlans, repoFeatures);
        var result = await service.CreateAsync(new CreatePlanDto { Code = "pro", Name = "Pro", PriceMonthly = 10, PriceYearly = 100 });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Pro", result.Value.Name);
    }
}