using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class OrganizationInvestmentPanel : BaseEntityModel, IAuditableEntity
{
    public int OrganizationProfileId { get; set; }

    public OrganizationProfile OrganizationProfile { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public decimal? MinimumInvestment { get; set; }

    public string ProfitShareModel { get; set; } = string.Empty;
}