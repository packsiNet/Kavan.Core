using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class MediaFile : BaseEntityModel, IAuditableEntity
{
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int? DurationSeconds { get; set; }
    public byte MediaFileTypeValue { get; set; }
    public bool IsStreamOnly { get; set; } = true;
}

