namespace ApplicationLayer.Dto.IdeaRatings;

public class IdeaRatingDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; }
}
