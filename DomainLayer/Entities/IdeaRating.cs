using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class IdeaRating : BaseEntityModel, IAuditableEntity
{
    public int IdeaId { get; set; }
    public Idea Idea { get; set; }

    public int UserId { get; set; }
    public UserAccount User { get; set; }

    public int Rating { get; set; }
}