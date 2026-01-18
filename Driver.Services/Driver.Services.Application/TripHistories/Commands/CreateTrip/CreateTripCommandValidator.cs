using FluentValidation;

namespace Driver.Services.Application.TripHistories.Commands.CreateTrip;

public class CreateTripCommandValidator : AbstractValidator<CreateTripCommand>
{
    public CreateTripCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId is required.")
            .MaximumLength(50)
            .WithMessage("OrderId must not exceed 50 characters.");

        RuleFor(x => x.PickupAddress)
            .NotEmpty()
            .WithMessage("Pickup address is required.")
            .MaximumLength(500)
            .WithMessage("Pickup address must not exceed 500 characters.");

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty()
            .WithMessage("Delivery address is required.")
            .MaximumLength(500)
            .WithMessage("Delivery address must not exceed 500 characters.");

        RuleFor(x => x.Fare).GreaterThan(0).WithMessage("Fare must be greater than zero.");

        RuleFor(x => x.CustomerNotes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.CustomerNotes))
            .WithMessage("Customer notes must not exceed 1000 characters.");
    }
}
