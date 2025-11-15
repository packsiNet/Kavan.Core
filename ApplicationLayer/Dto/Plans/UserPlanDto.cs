namespace ApplicationLayer.DTOs.Plans;

public class UserPlanDto
{
    public int Id { get; set; }

    public int UserAccountId { get; set; }

    public int PlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool AutoRenew { get; set; }
}