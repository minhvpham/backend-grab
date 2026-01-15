using FluentValidation;

namespace Driver.Services.Application.DriverWallets.Commands.CollectCash;

public class CollectCashCommandValidator : AbstractValidator<CollectCashCommand>
{
    public CollectCashCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage("DriverId is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.")
            .MaximumLength(50).WithMessage("OrderId must not exceed 50 characters.");

        RuleFor(x => x.Reference)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Reference))
            .WithMessage("Reference must not exceed 100 characters.");
    }
}
