using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class CourseCategory : BaseEntityModel, IAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Course> Courses { get; set; } = [];
}

