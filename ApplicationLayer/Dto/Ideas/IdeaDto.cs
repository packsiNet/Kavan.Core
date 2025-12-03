namespace ApplicationLayer.DTOs.Ideas;

public class IdeaDto
{
    public int Id { get; set; }
    public string Symbol { get; set; }
    public string Timeframe { get; set; }
    public string Trend { get; set; }
    public string Title { get; set; }
    public string TitleTranslate { get; set; }
    public string Description { get; set; }
    public string DescriptionTranslate { get; set; }
    public string Image { get; set; }
    public string Status { get; set; }
    public List<string> Tags { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
