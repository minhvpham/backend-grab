namespace Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;

public enum TransactionType
{
    OrderEarning = 1,
    Withdrawal = 2,
    Deposit = 3,
    Refund = 4,
    Penalty = 5,
    Bonus = 6,
    CashCollected = 7,
    CashReturned = 8
}
