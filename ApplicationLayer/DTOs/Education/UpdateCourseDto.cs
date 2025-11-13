namespace ApplicationLayer.DTOs.Education;

public class UpdateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsFree { get; set; }
    public byte CourseLevelValue { get; set; }
    public int CategoryId { get; set; }
}

