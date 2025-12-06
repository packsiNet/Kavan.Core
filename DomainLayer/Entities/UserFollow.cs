using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class UserFollow : BaseEntityModel, IAuditableEntity
{
    public int FollowerUserId { get; set; }

    public int FolloweeUserId { get; set; }

    public UserAccount FollowerUser { get; set; }

    public UserAccount FolloweeUser { get; set; }
}

