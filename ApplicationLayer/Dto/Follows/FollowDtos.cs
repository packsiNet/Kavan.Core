namespace ApplicationLayer.DTOs.Follows;

public class FollowSummaryDto
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string DisplayName { get; set; }
    public string Avatar { get; set; }
}

public class FollowStatusDto
{
    public bool IsFollowing { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
}

