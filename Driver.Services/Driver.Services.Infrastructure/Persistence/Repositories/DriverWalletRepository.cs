using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using Microsoft.EntityFrameworkCore;

namespace Driver.Services.Infrastructure.Persistence.Repositories;

public class DriverWalletRepository : IDriverWalletRepository
{
    private readonly DriverServicesDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public DriverWalletRepository(DriverServicesDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<DriverWallet?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverWallets
            .FirstOrDefaultAsync(dw => dw.Id == id, cancellationToken);
    }

    public async Task<DriverWallet?> GetByDriverIdAsync(
        Guid driverId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverWallets
            .FirstOrDefaultAsync(dw => dw.DriverId == driverId, cancellationToken);
    }

    public async Task<IReadOnlyList<DriverWallet>> GetWalletsWithBalanceAsync(
        decimal minBalance,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverWallets
            .Where(dw => dw.Balance >= minBalance && dw.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DriverWallet>> GetWalletsWithCashOnHandAsync(
        decimal minCashOnHand,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverWallets
            .Where(dw => dw.CashOnHand >= minCashOnHand && dw.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalBalanceAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverWallets
            .Where(dw => dw.IsActive)
            .SumAsync(dw => dw.Balance, cancellationToken);
    }

    public async Task<decimal> GetTotalCashOnHandAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverWallets
            .Where(dw => dw.IsActive)
            .SumAsync(dw => dw.CashOnHand, cancellationToken);
    }

    public DriverWallet Add(DriverWallet wallet)
    {
        return _context.DriverWallets.Add(wallet).Entity;
    }

    public void Update(DriverWallet wallet)
    {
        _context.DriverWallets.Update(wallet);
    }

    public void Remove(DriverWallet wallet)
    {
        _context.DriverWallets.Remove(wallet);
    }
}
