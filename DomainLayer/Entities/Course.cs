using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Course : BaseEntityModel, IAuditableEntity
{
    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Goal { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsFree { get; set; }

    public byte CourseLevelValue { get; set; }

    public int CategoryId { get; set; }

    public CourseCategory Category { get; set; } = null!;

    public int? OwnerUserId { get; set; }

    public UserAccount OwnerUser { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = [];
}