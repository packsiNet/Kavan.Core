using ApplicationLayer.Interfaces;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;

namespace InfrastructureLayer.BusinessLogic.Services.Identity;

[InjectAsScoped]
public class RoleSeedService(
    IRepository<Role> roles,
    IUnitOfWork uow) : IRoleSeedService
{
    public async Task SeedRolesAsync()
    {
        var names = new[] { "Admin", "User" };
        foreach (var name in names)
        {
            var exists = roles.Query().Any(r => r.RoleName == name);
            if (!exists)
            {
                await roles.AddAsync(new Role { RoleName = name });
                await uow.SaveChangesAsync();
            }
        }
        await uow.CommitAsync();
    }
}