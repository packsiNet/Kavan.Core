using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Notification : BaseEntityModel, IAuditableEntity
{
    public int UserAccountId { get; set; }

    public string Title { get; set; }

    public string Message { get; set; }

    public bool IsRead { get; set; }

    public UserAccount UserAccount { get; set; }
}