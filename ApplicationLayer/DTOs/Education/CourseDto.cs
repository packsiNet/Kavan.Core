namespace ApplicationLayer.DTOs.Education;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsFree { get; set; }
    public byte CourseLevelValue { get; set; }
    public int CategoryId { get; set; }
    public int OwnerUserId { get; set; }
}

