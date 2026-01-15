namespace Driver.Services.Application.DriverWallets.DTOs;

public class WalletBalanceDto
{
    public string WalletId { get; set; } = string.Empty;
    public string DriverId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public decimal CashOnHand { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public DateTime? LastWithdrawalAt { get; set; }
    public bool IsActive { get; set; }
}
