using ApplicationLayer.Common;
using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.DTOs.User;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace InfrastructureLayer.BusinessLogic.Services;

[InjectAsScoped]
public class UserAccountServices(IRepository<UserAccount> userAccountRepository, IRepository<UserProfile> userProfileRepository,
    IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository, IUserContextService userContextService,
    ILogger<UserAccountServices> logger, IMapper mapper) : IUserAccountServices
{
    public async Task<UserAccount> GetUserAccountByIdAsync(UserAccountKeyDto userAccount)
        => await Task.Run(() => userAccountRepository.GetDbSet().FirstOrDefaultAsync(row => row.Id == userAccount.Id));

    public ServiceResult GetUserByValidationMethodAsync(SignInDto signInDto)
    {
        try
        {
            var result = userAccountRepository.Query();

            if (signInDto.ValidationMethod == ValidationMethodEnum.OneTimePasswordEmail)
                result = result.Where(current => current.Email == signInDto.UserName);
            else if (signInDto.ValidationMethod == ValidationMethodEnum.OneTimePasswordMobile)
                result = result.Where(current => current.PhoneNumber == signInDto.UserName);
            else
                result = result.Where(
                    current => current.UserName == PhoneNumberHelper.NormalizePhoneNumber(signInDto.PhonePrefix, signInDto.UserName));

            if (!result?.Any() ?? true)
            {
                return new ServiceResult
                {
                    RequestStatus = RequestStatus.IncorrectUser,
                    Data = null,
                    Message = CommonMessages.IncorrectUser
                };
            }

            return new ServiceResult
            {
                RequestStatus = RequestStatus.Successful,
                Data = result.FirstOrDefault(),
                Message = CommonMessages.Successful
            };
        }
        catch (Exception exception)
        {
            logger.LogError(message: exception.Message, CommonMessages.Failed);

            return new ServiceResult
            {
                RequestStatus = RequestStatus.Failed,
                Data = null,
                Message = CommonMessages.Failed
            };
        }
    }

    public async Task<Result<UserAccount>> AddUserAccountAsync(UserAccount model)
    {
        try
        {
            model.SecurityCode = GenerateSecurityCode();
            model.ExpireSecurityCode = DateTime.Now.AddMinutes(10);

            var existUser = false;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                existUser = userAccountRepository.Query()
                    .Any(current => current.UserName == model.PhoneNumber || current.PhoneNumber == model.PhoneNumber);

            if (existUser)
                return Result<UserAccount>.DuplicateFailure();

            await userAccountRepository.AddAsync(model);
            return Result<UserAccount>.Success(model);
        }
        catch (Exception exception)
        {
            logger.LogError(message: exception.Message, CommonMessages.Failed);
            return Result<UserAccount>.GeneralFailure(CommonMessages.Failed);
        }
    }

    public async Task<ServiceResult> AddProfileAsync(UserProfile model)
    {
        try
        {
            model.IsActive = true;
            await userProfileRepository.AddAsync(model);
            return new ServiceResult { RequestStatus = RequestStatus.Successful, Data = model, Message = CommonMessages.Successful };
        }
        catch (Exception exception)
        {
            logger.LogError(message: exception.Message, CommonMessages.Failed);
            return new ServiceResult { RequestStatus = RequestStatus.Failed, Data = model, Message = CommonMessages.Failed };
        }
    }

    public async Task<ServiceResult> UpdateUserProfileAsync(UpdateUserProfileDto model)
    {
        try
        {
            var profileExists = await userProfileRepository.Query().FirstOrDefaultAsync(x => x.UserAccountId == model.Id);

            if (profileExists == null)
                return new ServiceResult { RequestStatus = RequestStatus.IncorrectUser, Message = CommonMessages.IncorrectUser };

            mapper.Map(model, profileExists);
            await userProfileRepository.UpdateAsync(profileExists);

            return new ServiceResult().Successful();
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(logger, excepotion, CommonExceptionMessage.AddFailed("آپدیت پروفایل"));
        }
    }

    public async Task<ServiceResult> AssignRoleToUserAsync(UserAccountKeyDto userAccountId, string roleName)
    {
        try
        {
            var role = await roleRepository.Query()
                .FirstOrDefaultAsync(r => r.RoleName == roleName);

            if (role == null)
            {
                return new ServiceResult
                {
                    RequestStatus = RequestStatus.NotFound,
                    Message = $"نقش {roleName} یافت نشد."
                };
            }

            var userRole = new UserRole
            {
                UserAccountId = userAccountId.Id,
                RoleId = role.Id
            };

            await userRoleRepository.AddAsync(userRole);

            return new ServiceResult().Successful();
        }
        catch (Exception exception)
        {
            return new ServiceResult().Failed(logger, exception, CommonExceptionMessage.AddFailed("تخصیص نقش"));
        }
    }

    public async Task<ServiceResult> UserInfoAsync()
    {
        try
        {
            UserAccountKeyDto userAccount = new() { Id = userContextService.UserId.Value };
            var user = await GetUserAccountByIdAsync(userAccount);
            if (user == null)
                return new ServiceResult().NotFound();

            var userInfo = new UserInfoDto
            {
                UserAccountId = user.Id,
                DisplayName = user.UserProfiles.FirstOrDefault()?.DisplayName,
                FirstName = user.UserProfiles.FirstOrDefault()?.FirstName,
                LastName = user.UserProfiles.FirstOrDefault()?.LastName,
            };

            return new ServiceResult().Successful(userInfo);
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(logger, excepotion, CommonExceptionMessage.GetFailed("اطلاعات کاربر"));
        }
    }

    private static int GenerateSecurityCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        int number = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;
        return 10000 + (number % 90000);
    }
}