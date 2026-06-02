using CareNota.DTOs.Summary;
using FluentValidation;

namespace CareNota.Validators;

public class RateSummaryValidator : AbstractValidator<RateSummaryDto>
{
    public RateSummaryValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Feedback)
            .MaximumLength(1000)
            .WithMessage("Feedback cannot exceed 1000 characters.")
            .When(x => x.Feedback is not null);
    }
}