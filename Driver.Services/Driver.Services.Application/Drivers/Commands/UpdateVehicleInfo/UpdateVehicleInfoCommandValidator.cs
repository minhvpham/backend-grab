using FluentValidation;

namespace Driver.Services.Application.Drivers.Commands.UpdateVehicleInfo;

public class UpdateVehicleInfoCommandValidator : AbstractValidator<UpdateVehicleInfoCommand>
{
    public UpdateVehicleInfoCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty()
            .WithMessage("Driver ID is required.");

        RuleFor(x => x.VehicleType)
            .NotEmpty()
            .WithMessage("Vehicle type is required.")
            .MaximumLength(50)
            .WithMessage("Vehicle type must not exceed 50 characters.");

        RuleFor(x => x.PlateNumber)
            .NotEmpty()
            .WithMessage("Plate number is required.")
            .MaximumLength(20)
            .WithMessage("Plate number must not exceed 20 characters.");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage("Vehicle brand is required.")
            .MaximumLength(50)
            .WithMessage("Vehicle brand must not exceed 50 characters.");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Vehicle model is required.")
            .MaximumLength(50)
            .WithMessage("Vehicle model must not exceed 50 characters.");

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Vehicle color is required.")
            .MaximumLength(30)
            .WithMessage("Vehicle color must not exceed 30 characters.");

        RuleFor(x => x.Year)
            .GreaterThan(1900)
            .WithMessage("Vehicle year must be after 1900.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1)
            .WithMessage($"Vehicle year cannot be more than {DateTime.UtcNow.Year + 1}.");
    }
}
