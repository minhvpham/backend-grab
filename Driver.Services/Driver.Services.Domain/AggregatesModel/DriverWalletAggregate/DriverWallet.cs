using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.Exceptions;

namespace Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;

public class DriverWallet : Entity<string>, IAggregateRoot
{
    public string DriverId { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public decimal CashOnHand { get; private set; } // COD amount driver collected but not settled
    public decimal TotalEarnings { get; private set; }
    public decimal TotalWithdrawn { get; private set; }
    public DateTime? LastWithdrawalAt { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    private DriverWallet() { } // For EF Core

    public static DriverWallet Create(string driverId)
    {
        var wallet = new DriverWallet
        {
            Id = Guid.NewGuid().ToString(),
            DriverId = driverId,
            Balance = 0,
            CashOnHand = 0,
            TotalEarnings = 0,
            TotalWithdrawn = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return wallet;
    }

    // Add earnings from completed order
    public void AddOrderEarning(decimal amount, string orderId, string? description = null)
    {
        if (amount <= 0)
            throw new DomainValidationException("Order earning amount must be greater than zero");

        var transaction = new Transaction(
            TransactionType.OrderEarning,
            amount,
            Balance,
            orderId: orderId,
            description: description ?? $"Earnings from order {orderId}");

        _transactions.Add(transaction);
        Balance += amount;
        TotalEarnings += amount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WalletBalanceChangedDomainEvent(Id, DriverId, Balance, TransactionType.OrderEarning, amount));
    }

    // Record COD collection - deducts from balance, adds to cash on hand
    public void RecordCashCollection(decimal amount, string orderId, string? reference = null)
    {
        if (amount <= 0)
            throw new DomainValidationException("Cash collection amount must be greater than zero");

        if (Balance < amount)
            throw new DomainValidationException($"Insufficient balance. Current: {Balance}, Required: {amount}");

        var transaction = new Transaction(
            TransactionType.CashCollected,
            amount,
            Balance,
            orderId: orderId,
            reference: reference,
            description: $"COD collected from order {orderId}");

        _transactions.Add(transaction);
        Balance -= amount;
        CashOnHand += amount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WalletCashCollectedDomainEvent(Id, DriverId, amount, orderId));
    }

    // Return cash to wallet balance (deposit COD)
    public void ReturnCash(decimal amount, string? reference = null, string? description = null)
    {
        if (amount <= 0)
            throw new DomainValidationException("Return amount must be greater than zero");

        if (CashOnHand < amount)
            throw new DomainValidationException($"Insufficient cash on hand. Current: {CashOnHand}, Required: {amount}");

        var transaction = new Transaction(
            TransactionType.CashReturned,
            amount,
            Balance,
            reference: reference,
            description: description ?? "Cash returned to balance");

        _transactions.Add(transaction);
        Balance += amount;
        CashOnHand -= amount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WalletCashReturnedDomainEvent(Id, DriverId, amount, Balance, reference));
    }

    // Withdraw from balance
    public void Withdraw(decimal amount, string? reference = null, string? description = null)
    {
        if (amount <= 0)
            throw new DomainValidationException("Withdrawal amount must be greater than zero");

        if (amount > Balance)
            throw new DomainValidationException($"Insufficient balance. Current: {Balance}, Required: {amount}");

        var transaction = new Transaction(
            TransactionType.Withdrawal,
            amount,
            Balance,
            reference: reference,
            description: description ?? $"Withdrawal: {amount:C}");

        _transactions.Add(transaction);
        Balance -= amount;
        TotalWithdrawn += amount;
        LastWithdrawalAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WalletBalanceChangedDomainEvent(Id, DriverId, Balance, TransactionType.Withdrawal, amount));
    }

    // Deposit to balance
    public void Deposit(decimal amount, string? reference = null, string? description = null)
    {
        if (amount <= 0)
            throw new DomainValidationException("Deposit amount must be greater than zero");

        var transaction = new Transaction(
            TransactionType.Deposit,
            amount,
            Balance,
            reference: reference,
            description: description ?? $"Deposit: {amount:C}");

        _transactions.Add(transaction);
        Balance += amount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WalletBalanceChangedDomainEvent(Id, DriverId, Balance, TransactionType.Deposit, amount));
    }

    // Apply bonus
    public void AddBonus(decimal amount, string? description = null, string? reference = null)
    {
        if (amount <= 0)
            throw new DomainValidationException("Bonus amount must be greater than zero");

        var transaction = new Transaction(
            TransactionType.Bonus,
            amount,
            Balance,
            reference: reference ?? $"BONUS-{Guid.NewGuid():N}",
            description: description ?? "Bonus added");

        _transactions.Add(transaction);
        Balance += amount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WalletBalanceChangedDomainEvent(Id, DriverId, Balance, TransactionType.Bonus, amount));
    }

    // Apply penalty
    public void ApplyPenalty(decimal amount, string reason, string? reference = null)
    {
        if (amount <= 0)
            throw new DomainValidationException("Penalty amount must be greater than zero");

        var transaction = new Transaction(
            TransactionType.Penalty,
            amount,
            Balance,
            reference: reference ?? $"PENALTY-{Guid.NewGuid():N}",
            description: reason);

        _transactions.Add(transaction);
        Balance -= amount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WalletBalanceChangedDomainEvent(Id, DriverId, Balance, TransactionType.Penalty, amount));
    }

    // Refund transaction
    public void RefundTransaction(decimal amount, string orderId, string? reason = null, string? reference = null)
    {
        if (amount <= 0)
            throw new DomainValidationException("Refund amount must be greater than zero");

        var transaction = new Transaction(
            TransactionType.Refund,
            amount,
            Balance,
            orderId: orderId,
            reference: reference,
            description: reason ?? $"Refund for order {orderId}");

        _transactions.Add(transaction);
        Balance += amount;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new WalletBalanceChangedDomainEvent(Id, DriverId, Balance, TransactionType.Refund, amount));
    }

    // Deactivate wallet
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // Reactivate wallet
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // Get all transactions
    public IReadOnlyList<Transaction> GetTransactions() => _transactions.AsReadOnly();

    // Get recent transactions
    public IReadOnlyList<Transaction> GetRecentTransactions(int count = 10)
    {
        return _transactions
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .ToList()
            .AsReadOnly();
    }

    // Get transactions by type
    public IReadOnlyList<Transaction> GetTransactionsByType(TransactionType type)
    {
        return _transactions
            .Where(t => t.Type == type)
            .OrderByDescending(t => t.CreatedAt)
            .ToList()
            .AsReadOnly();
    }
}
