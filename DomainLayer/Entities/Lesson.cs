using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Lesson : BaseEntityModel, IAuditableEntity
{
    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Order { get; set; }

    public DateTime? PublishAt { get; set; }

    public bool IsFreePreview { get; set; }

    public int? DurationSeconds { get; set; }

    public ICollection<MediaFile> MediaFiles { get; set; } = [];
}