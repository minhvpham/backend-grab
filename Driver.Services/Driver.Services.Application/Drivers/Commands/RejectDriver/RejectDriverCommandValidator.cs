using FluentValidation;

namespace Driver.Services.Application.Drivers.Commands.RejectDriver;

public class RejectDriverCommandValidator : AbstractValidator<RejectDriverCommand>
{
    public RejectDriverCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty()
            .WithMessage("Driver ID is required.");

        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .WithMessage("Rejection reason is required.")
            .MinimumLength(10)
            .WithMessage("Rejection reason must be at least 10 characters.")
            .MaximumLength(500)
            .WithMessage("Rejection reason must not exceed 500 characters.");
    }
}
