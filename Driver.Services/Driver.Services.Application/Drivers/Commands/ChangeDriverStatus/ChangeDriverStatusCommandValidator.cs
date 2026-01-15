using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using FluentValidation;

namespace Driver.Services.Application.Drivers.Commands.ChangeDriverStatus;

public class ChangeDriverStatusCommandValidator : AbstractValidator<ChangeDriverStatusCommand>
{
    public ChangeDriverStatusCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty()
            .WithMessage("Driver ID is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid driver status value.");
    }
}
