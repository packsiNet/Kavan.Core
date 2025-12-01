using ApplicationLayer.Common;
using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.Interfaces;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InfrastructureLayer.BusinessLogic.Services;

[InjectAsScoped]
public class IdentityService(IConfiguration _iConfiguration,
                             IHttpContextAccessor _httpContextAccessor,
                             IRepository<Role> _roleRepository,
                             IRepository<UserRole> _userRoleRepository) : IIdentityService
{
    public ServiceResult AuthenticateOneTimePassword(SignInDto signInViewModel, UserAccount userAccount)
    {
        if (signInViewModel.SecurityCode != userAccount.SecurityCode || DateTime.Now > userAccount.ExpireSecurityCode)
        {
            return new ServiceResult
            {
                RequestStatus = RequestStatus.NotFound,
                Data = signInViewModel,
                Message = IdentityMessages.IncorrectSecurityCode
            };
        }

        AuthorizeResultDto result = new()
        {
            UserFullName = signInViewModel.UserName
        };

        return new ServiceResult
        {
            RequestStatus = RequestStatus.Successful,
            Data = result,
            Message = CommonMessages.Successful
        };
    }

    public async Task<ServiceResult> AuthenticateUserInformationAsync(SignInDto signInViewModel, UserAccount userAccount)
    {
        var passwordIsValid = HashGenerator.VerifyPassword(signInViewModel.Password, userAccount.Password, userAccount.SecurityStamp);
        if (!passwordIsValid)
            return new ServiceResult { RequestStatus = RequestStatus.IncorrectUser, Message = IdentityMessages.IncorrectPassword };

        var roles = await _userRoleRepository.Query()
            .Where(ur => ur.UserAccountId == userAccount.Id)
            .Join(_roleRepository.Query(), ur => ur.RoleId, r => r.Id, (ur, r) => r.RoleName)
            .ToListAsync();

        var (accessToken, tokenId) = TokenGenerator(userAccount.UserName, userAccount.Id, roles);

        AuthorizeResultDto result = new()
        {
            AccessTokens = accessToken,
            TokenId = tokenId,
            UserFullName = userAccount.UserName
        };

        return new ServiceResult
        {
            RequestStatus = RequestStatus.Successful,
            Data = result,
            Message = CommonMessages.Successful
        };
    }

    public (string jwtToken, string tokenId) TokenGenerator(string userName, int userId, List<string> roles)
    {
        if (_iConfiguration == null) throw new ArgumentNullException(nameof(_iConfiguration));
        if (_httpContextAccessor == null) throw new ArgumentNullException(nameof(_httpContextAccessor));

        var tokenHandler = new JwtSecurityTokenHandler();
        var keyString = _iConfiguration["JWT:Key"] ?? Environment.GetEnvironmentVariable("JWT__Key");
        if (string.IsNullOrWhiteSpace(keyString)) throw new InvalidOperationException("JWT:Key is not configured");
        var tokenKey = Encoding.UTF8.GetBytes(keyString);
        if (tokenKey.Length < 32) throw new ArgumentException("JWT key must be at least 256 bits (32 bytes) long.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expStr = _iConfiguration["JWT:JwtTokenExpirationTimeInMinutes"] ?? Environment.GetEnvironmentVariable("JWT__JwtTokenExpirationTimeInMinutes");
        if (!double.TryParse(expStr, out var expMinutes)) expMinutes = 15;
        var issuer = _iConfiguration["JWT:Issuer"] ?? Environment.GetEnvironmentVariable("JWT__Issuer") ?? "Kavan";
        var audience = _iConfiguration["JWT:Audience"] ?? Environment.GetEnvironmentVariable("JWT__Audience") ?? "Kavan.Users";

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return (jwtToken, token.Id);
    }

    public async Task<(string jwtToken, string tokenId)> TokenRequestGeneratorAsync(string userName, int userAccountId)
    {
        if (_iConfiguration == null) throw new ArgumentNullException(nameof(_iConfiguration));
        if (_httpContextAccessor == null) throw new ArgumentNullException(nameof(_httpContextAccessor));

        var roles = await _userRoleRepository.Query()
            .Where(ur => ur.UserAccountId == userAccountId)
            .Join(_roleRepository.Query(), ur => ur.RoleId, r => r.Id, (ur, r) => r.RoleName)
            .ToListAsync();

        var tokenHandler = new JwtSecurityTokenHandler();
        var keyString = _iConfiguration["JWT:Key"] ?? Environment.GetEnvironmentVariable("JWT__Key");
        if (string.IsNullOrWhiteSpace(keyString)) throw new InvalidOperationException("JWT:Key is not configured");
        var tokenKey = Encoding.UTF8.GetBytes(keyString);
        if (tokenKey.Length < 32) throw new ArgumentException("JWT key must be at least 256 bits (32 bytes) long.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.NameIdentifier, userAccountId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expStr = _iConfiguration["JWT:JwtTokenExpirationTimeInMinutes"] ?? Environment.GetEnvironmentVariable("JWT__JwtTokenExpirationTimeInMinutes");
        if (!double.TryParse(expStr, out var expMinutes)) expMinutes = 15;
        var issuer = _iConfiguration["JWT:Issuer"] ?? Environment.GetEnvironmentVariable("JWT__Issuer") ?? "Kavan";
        var audience = _iConfiguration["JWT:Audience"] ?? Environment.GetEnvironmentVariable("JWT__Audience") ?? "Kavan.Users";

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return (jwtToken, token.Id);
    }
}