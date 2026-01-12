namespace Driver.Services.Application.DriverWallets.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? OrderId { get; set; }
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsCredit { get; set; }
}
