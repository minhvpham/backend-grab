using FluentValidation;

namespace Driver.Services.Application.Drivers.Queries.GetDriverById;

public class GetDriverByIdQueryValidator : AbstractValidator<GetDriverByIdQuery>
{
    public GetDriverByIdQueryValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty()
            .WithMessage("Driver ID is required.");
    }
}
