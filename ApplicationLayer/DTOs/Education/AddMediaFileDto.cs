namespace ApplicationLayer.DTOs.Education;

public class AddMediaFileDto
{
    public int LessonId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int? DurationSeconds { get; set; }
    public byte MediaFileTypeValue { get; set; }
}

