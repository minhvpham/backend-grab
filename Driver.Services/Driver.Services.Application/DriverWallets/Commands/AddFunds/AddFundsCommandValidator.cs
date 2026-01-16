using FluentValidation;

namespace Driver.Services.Application.DriverWallets.Commands.AddFunds;

public class AddFundsCommandValidator : AbstractValidator<AddFundsCommand>
{
    public AddFundsCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage("DriverId is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Reference)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Reference))
            .WithMessage("Reference must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description must not exceed 500 characters.");
    }
}
