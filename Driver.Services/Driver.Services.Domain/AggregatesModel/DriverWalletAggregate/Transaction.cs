using Driver.Services.Domain.Exceptions;

namespace Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;

public class Transaction
{
    public Guid Id { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public decimal BalanceBefore { get; private set; }
    public decimal BalanceAfter { get; private set; }
    public string? OrderId { get; private set; }
    public string? Reference { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() { } // For EF Core

    internal Transaction(
        TransactionType type,
        decimal amount,
        decimal balanceBefore,
        string? orderId = null,
        string? reference = null,
        string? description = null)
    {
        if (amount <= 0)
            throw new DomainValidationException("Transaction amount must be greater than zero");

        Id = Guid.NewGuid();
        Type = type;
        Amount = amount;
        BalanceBefore = balanceBefore;
        BalanceAfter = type switch
        {
            TransactionType.OrderEarning or 
            TransactionType.Deposit or 
            TransactionType.Bonus or 
            TransactionType.CashReturned or 
            TransactionType.Refund => balanceBefore + amount,
            TransactionType.Withdrawal or 
            TransactionType.Penalty or 
            TransactionType.CashCollected => balanceBefore - amount,
            _ => balanceBefore
        };
        OrderId = orderId;
        Reference = reference;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsCredit() => Type switch
    {
        TransactionType.OrderEarning or
        TransactionType.Deposit or
        TransactionType.Bonus or
        TransactionType.CashReturned or
        TransactionType.Refund => true,
        _ => false
    };

    public bool IsDebit() => !IsCredit();
}
