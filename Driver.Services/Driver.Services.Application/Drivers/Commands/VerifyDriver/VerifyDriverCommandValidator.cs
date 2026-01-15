using FluentValidation;

namespace Driver.Services.Application.Drivers.Commands.VerifyDriver;

public class VerifyDriverCommandValidator : AbstractValidator<VerifyDriverCommand>
{
    public VerifyDriverCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty()
            .WithMessage("Driver ID is required.");
    }
}
