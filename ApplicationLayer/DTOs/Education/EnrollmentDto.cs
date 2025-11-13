namespace ApplicationLayer.DTOs.Education;

public class EnrollmentDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int UserAccountId { get; set; }
    public bool IsPaid { get; set; }
    public decimal PricePaid { get; set; }
    public string? CouponCode { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime EnrolledAt { get; set; }
}

