namespace ApplicationLayer.DTOs.Plans;

public class UpdatePlanDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal PriceMonthly { get; set; }
    public decimal PriceYearly { get; set; }
    public bool IsPublic { get; set; }
    public bool IsActive { get; set; }
    public List<PlanFeatureDto> Features { get; set; } = [];
}