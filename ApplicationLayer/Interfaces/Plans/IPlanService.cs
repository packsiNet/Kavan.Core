using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Plans;

namespace ApplicationLayer.Interfaces.Services;

public interface IPlanService
{
    Task<Result<PlanDto>> CreateAsync(CreatePlanDto dto);

    Task<Result<PlanDto>> UpdateAsync(int id, UpdatePlanDto dto);

    Task<Result> DeleteAsync(int id);

    Task<Result<PlanDto>> GetByIdAsync(int id);

    Task<Result<List<PlanDto>>> GetAllAsync();
}