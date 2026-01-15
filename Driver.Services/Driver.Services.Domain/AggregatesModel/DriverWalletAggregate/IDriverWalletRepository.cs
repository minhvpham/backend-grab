using Driver.Services.Domain.Abstractions;

namespace Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;

public interface IDriverWalletRepository : IRepository<DriverWallet>
{
    Task<DriverWallet?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<DriverWallet?> GetByDriverIdAsync(string driverId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DriverWallet>> GetWalletsWithBalanceAsync(
        decimal minBalance,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DriverWallet>> GetWalletsWithCashOnHandAsync(
        decimal minCashOnHand,
        CancellationToken cancellationToken = default);
    Task<decimal> GetTotalBalanceAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalCashOnHandAsync(CancellationToken cancellationToken = default);
    DriverWallet Add(DriverWallet wallet);
    void Update(DriverWallet wallet);
    void Remove(DriverWallet wallet);
}
