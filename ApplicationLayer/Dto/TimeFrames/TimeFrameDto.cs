namespace ApplicationLayer.Dto.TimeFrames;

public class TimeFrameDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameEnglish { get; set; } = string.Empty;
    public string NamePersian { get; set; } = string.Empty;
    public int DurationInMinutes { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public string Category { get; set; } = string.Empty;
}