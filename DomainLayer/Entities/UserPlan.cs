using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class UserPlan : BaseEntityModel, IAuditableEntity
{
    public int UserAccountId { get; set; }

    public int PlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool AutoRenew { get; set; }

    #region Navigations

    public UserAccount UserAccount { get; set; }

    public Plan Plan { get; set; }

    #endregion Navigations
}