using Driver.Services.Domain.Abstractions;

namespace Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;

// Base wallet balance changed event
public record WalletBalanceChangedDomainEvent(
    string WalletId,
    string DriverId,
    decimal NewBalance,
    TransactionType TransactionType,
    decimal Amount) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

// Cash collected from order
public record WalletCashCollectedDomainEvent(
    string WalletId,
    string DriverId,
    decimal Amount,
    string OrderId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

// Cash returned to balance
public record WalletCashReturnedDomainEvent(
    string WalletId,
    string DriverId,
    decimal Amount,
    decimal NewBalance,
    string? Reference) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
