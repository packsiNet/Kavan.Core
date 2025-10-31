using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Plan : BaseEntityModel, IAuditableEntity
{
    public string Name { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }

    public decimal PriceMonthly { get; set; }

    public decimal PriceYearly { get; set; }

    public bool IsPublic { get; set; }

    public ICollection<PlanFeature> Features { get; set; } = [];

    public ICollection<UserPlan> UserPlans { get; set; } = [];
}