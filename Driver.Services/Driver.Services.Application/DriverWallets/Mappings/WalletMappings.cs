using Driver.Services.Application.DriverWallets.DTOs;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;

namespace Driver.Services.Application.DriverWallets.Mappings;

public static class WalletMappings
{
    public static WalletBalanceDto ToBalanceDto(this DriverWallet wallet)
    {
        return new WalletBalanceDto
        {
            WalletId = wallet.Id,
            DriverId = wallet.DriverId,
            Balance = wallet.Balance,
            CashOnHand = wallet.CashOnHand,
            TotalEarnings = wallet.TotalEarnings,
            TotalWithdrawn = wallet.TotalWithdrawn,
            LastWithdrawalAt = wallet.LastWithdrawalAt,
            IsActive = wallet.IsActive
        };
    }

    public static TransactionDto ToDto(this Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            Type = transaction.Type.ToString(),
            Amount = transaction.Amount,
            BalanceBefore = transaction.BalanceBefore,
            BalanceAfter = transaction.BalanceAfter,
            OrderId = transaction.OrderId,
            Reference = transaction.Reference,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            IsCredit = transaction.IsCredit()
        };
    }
}
