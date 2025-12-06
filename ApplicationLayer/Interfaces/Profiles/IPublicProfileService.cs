using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Profiles.Public;

namespace ApplicationLayer.Interfaces.Profiles;

public interface IPublicProfileService
{
    Task<Result<PublicProfileDto>> GetByUserIdAsync(int userId);
}
