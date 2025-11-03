using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class PlanFeature : BaseEntityModel, IAuditableEntity
{
    public int PlanId { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }

    public string Unit { get; set; }

    #region Navigations

    public Plan Plan { get; set; }

    #endregion Navigations
}