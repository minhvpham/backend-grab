using FluentValidation;

namespace Driver.Services.Application.TripHistories.Commands.CompleteTrip;

public class CompleteTripCommandValidator : AbstractValidator<CompleteTripCommand>
{
    public CompleteTripCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty().WithMessage("TripId is required.");

        RuleFor(x => x.CashCollected)
            .GreaterThanOrEqualTo(0).When(x => x.CashCollected.HasValue)
            .WithMessage("Cash collected cannot be negative.");

        RuleFor(x => x.DriverNotes)
            .MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.DriverNotes))
            .WithMessage("Driver notes must not exceed 1000 characters.");
    }
}
