using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.DTOs.User;
using DomainLayer.Entities;

namespace ApplicationLayer.Interfaces;

public interface IUserAccountServices
{
    Task<UserAccount> GetUserAccountByIdAsync(UserAccountKeyDto userAccount);

    ServiceResult GetUserByValidationMethodAsync(SignInDto signInViewModel);

    Task<Result<UserAccount>> AddUserAccountAsync(UserAccount model);

    Task<ServiceResult> AddProfileAsync(UserProfile model);

    Task<ServiceResult> UpdateUserProfileAsync(UpdateUserProfileDto dto);

    Task<ServiceResult> AssignRoleToUserAsync(UserAccountKeyDto userAccountId, string roleName);

    Task<ServiceResult> UserInfoAsync();
}