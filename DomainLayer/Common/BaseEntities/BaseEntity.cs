namespace DomainLayer.Common.BaseEntities;

public abstract class BaseEntity<T> : IBaseEntity<T>
{
    protected BaseEntity()
    {
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public virtual T Id { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        IsActive = false;
    }

    public void Restore()
    {
        IsDeleted = false;
        IsActive = true;
    }
}