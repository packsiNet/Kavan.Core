using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class CourseEnrollment : BaseEntityModel, IAuditableEntity
{
    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public int UserAccountId { get; set; }

    public UserAccount UserAccount { get; set; } = null!;

    public bool IsPaid { get; set; }

    public decimal PricePaid { get; set; }

    public string? CouponCode { get; set; }

    public string? PaymentReference { get; set; }

    public DateTime EnrolledAt { get; set; }
}