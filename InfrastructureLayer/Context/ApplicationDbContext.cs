#region Usings

using InfrastructureLayer.Configuration;
using InfrastructureLayer.Convention;
using Microsoft.EntityFrameworkCore;

#endregion

namespace InfrastructureLayer.Context
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructors

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #endregion Constructors

        #region ModelCreating & OnConfiguring

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Ignore<BaseInformationLocalizationProperty>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseEntityConfiguration<>).Assembly);
            modelBuilder.SetGlobalConvention();
            modelBuilder.AddAuditableShadowProperties();

            SetGlobalFilter.ApplyQueryFilters(modelBuilder);
        }

        #endregion ModelCreating & OnConfiguring
    }
}