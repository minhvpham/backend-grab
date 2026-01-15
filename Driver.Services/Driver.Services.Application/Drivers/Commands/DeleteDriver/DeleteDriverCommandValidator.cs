using FluentValidation;

namespace Driver.Services.Application.Drivers.Commands.DeleteDriver;

public class DeleteDriverCommandValidator : AbstractValidator<DeleteDriverCommand>
{
    public DeleteDriverCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage("Driver ID is required");
    }
}