namespace ApplicationLayer.Dto.IdeaRatings;

public class IdeaRatingStatsDto
{
    public int IdeaId { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public int? MyRating { get; set; }
}
