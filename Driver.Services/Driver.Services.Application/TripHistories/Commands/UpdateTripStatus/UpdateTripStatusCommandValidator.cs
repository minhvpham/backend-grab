using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using FluentValidation;

namespace Driver.Services.Application.TripHistories.Commands.UpdateTripStatus;

public class UpdateTripStatusCommandValidator : AbstractValidator<UpdateTripStatusCommand>
{
    public UpdateTripStatusCommandValidator()
    {
        RuleFor(x => x.TripId)
            .NotEmpty().WithMessage("TripId is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid trip status.");
    }
}
