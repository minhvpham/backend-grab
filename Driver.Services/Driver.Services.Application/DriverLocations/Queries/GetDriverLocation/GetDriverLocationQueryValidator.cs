using FluentValidation;

namespace Driver.Services.Application.DriverLocations.Queries.GetDriverLocation;

public class GetDriverLocationQueryValidator : AbstractValidator<GetDriverLocationQuery>
{
    public GetDriverLocationQueryValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty()
            .WithMessage("Driver ID is required.");
    }
}
