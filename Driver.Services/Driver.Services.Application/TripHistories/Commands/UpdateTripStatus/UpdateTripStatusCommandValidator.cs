using FluentValidation;

namespace Driver.Services.Application.TripHistories.Commands.UpdateTripStatus;

public class UpdateTripStatusCommandValidator : AbstractValidator<UpdateTripStatusCommand>
{
    public UpdateTripStatusCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty().WithMessage("TripId is required.");

        RuleFor(x => x.Action)
            .NotEmpty().WithMessage("Action is required.")
            .Must(a => new[] { "accept", "pickup", "start_delivery" }.Contains(a.ToLower()))
            .WithMessage("Action must be 'accept', 'pickup', or 'start_delivery'.");
    }
}
