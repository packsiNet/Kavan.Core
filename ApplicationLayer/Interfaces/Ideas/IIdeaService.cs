using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Ideas;

namespace ApplicationLayer.Interfaces.Ideas;

public interface IIdeaService
{
    Task<Result<IdeaDto>> CreateAsync(CreateIdeaDto dto);
    Task<Result<IdeaDto>> UpdateAsync(int id, UpdateIdeaDto dto);
    Task<Result> DeleteAsync(int id);
    Task<Result<IdeaDto>> GetByIdAsync(int id);
    Task<Result<IdeasPageDto>> GetPublicAsync(GetIdeasRequestDto dto);
    Task<Result<IdeasPageDto>> GetMineAsync(GetIdeasRequestDto dto);
    Task<Result<IdeasPageDto>> GetForUserAsync(int userId, GetIdeasRequestDto dto);
}