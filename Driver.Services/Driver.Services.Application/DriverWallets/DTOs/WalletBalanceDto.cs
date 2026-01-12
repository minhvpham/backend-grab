namespace Driver.Services.Application.DriverWallets.DTOs;

public class WalletBalanceDto
{
    public Guid WalletId { get; set; }
    public Guid DriverId { get; set; }
    public decimal Balance { get; set; }
    public decimal CashOnHand { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public DateTime? LastWithdrawalAt { get; set; }
    public bool IsActive { get; set; }
}
