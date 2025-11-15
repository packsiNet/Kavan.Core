using ApplicationLayer.Common;
using ApplicationLayer.DTOs.Education;
using FluentValidation;

namespace ApplicationLayer.Features.Education.Validations;

public class EducationValidators
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull).MaximumLength(200);
            RuleFor(x => x.Slug).NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull).MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(4000);
            RuleFor(x => x.Goal).MaximumLength(4000);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);
            RuleFor(x => x.CourseLevelValue).InclusiveBetween((byte)1, (byte)5);
            RuleFor(x => x.CategoryId).GreaterThan(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);
        }
    }

    public class UpdateCourseValidator : AbstractValidator<UpdateCourseDto>
    {
        public UpdateCourseValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull).MaximumLength(200);
            RuleFor(x => x.Slug).NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull).MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(4000);
            RuleFor(x => x.Goal).MaximumLength(4000);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);
            RuleFor(x => x.CourseLevelValue).InclusiveBetween((byte)1, (byte)5);
            RuleFor(x => x.CategoryId).GreaterThan(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);
        }
    }

    public class CreateLessonValidator : AbstractValidator<CreateLessonDto>
    {
        public CreateLessonValidator()
        {
            RuleFor(x => x.CourseId).GreaterThan(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);
            RuleFor(x => x.Title).NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull).MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(4000);
            RuleFor(x => x.Order).GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);
            RuleFor(x => x.DurationSeconds).GreaterThan(0).When(x => x.DurationSeconds.HasValue);
        }
    }

    public class AddMediaFileValidator : AbstractValidator<AddMediaFileDto>
    {
        public AddMediaFileValidator()
        {
            RuleFor(x => x.LessonId).GreaterThan(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);
            RuleFor(x => x.FileName).NotEmpty().MaximumLength(260);
            RuleFor(x => x.StorageKey).NotEmpty().MaximumLength(260);
            RuleFor(x => x.MimeType).NotEmpty().MaximumLength(100);
            RuleFor(x => x.SizeBytes).GreaterThan(0);
            RuleFor(x => x.MediaFileTypeValue).InclusiveBetween((byte)1, (byte)10);
        }
    }

    public class EnrollCourseValidator : AbstractValidator<EnrollCourseDto>
    {
        public EnrollCourseValidator()
        {
            RuleFor(x => x.CourseId).GreaterThan(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);
            RuleFor(x => x.PaymentReference).NotEmpty().When(x => x.CouponCode == null);
        }
    }

    public class SetCoursePricingValidator : AbstractValidator<SetCoursePricingDto>
    {
        public SetCoursePricingValidator()
        {
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero);
        }
    }

    public class ScheduleLessonValidator : AbstractValidator<ScheduleLessonDto>
    {
        public ScheduleLessonValidator()
        {
            // PublishAt optional; no hard rule needed here
        }
    }
}