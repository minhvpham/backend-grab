using FluentValidation;

namespace Driver.Services.Application.Drivers.Commands.RegisterDriver;

public class RegisterDriverCommandValidator : AbstractValidator<RegisterDriverCommand>
{
    public RegisterDriverCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(200).WithMessage("Full name must not exceed 200 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Phone number must be between 10 and 15 digits");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number is required")
            .MaximumLength(50).WithMessage("License number must not exceed 50 characters");

        RuleFor(x => x.DriverId)
            .Must(id => id == null || !string.IsNullOrWhiteSpace(id)).WithMessage("Driver ID cannot be empty or whitespace");

        RuleFor(x => x.CitizenIdImageUrl)
            .MaximumLength(500).WithMessage("Citizen ID image URL must not exceed 500 characters")
            .Must(url => url == null || Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("Citizen ID image URL must be a valid URL");

        RuleFor(x => x.DriverLicenseImageUrl)
            .MaximumLength(500).WithMessage("Driver license image URL must not exceed 500 characters")
            .Must(url => url == null || Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("Driver license image URL must be a valid URL");

        RuleFor(x => x.DriverRegistrationImageUrl)
            .MaximumLength(500).WithMessage("Driver registration image URL must not exceed 500 characters")
            .Must(url => url == null || Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("Driver registration image URL must be a valid URL");
    }
}
