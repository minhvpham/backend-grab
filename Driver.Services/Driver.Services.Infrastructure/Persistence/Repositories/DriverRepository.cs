using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Microsoft.EntityFrameworkCore;

namespace Driver.Services.Infrastructure.Persistence.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly DriverServicesDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public DriverRepository(DriverServicesDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Domain.AggregatesModel.DriverAggregate.Driver?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Drivers
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Domain.AggregatesModel.DriverAggregate.Driver?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Drivers
            .FirstOrDefaultAsync(d => d.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<Domain.AggregatesModel.DriverAggregate.Driver?> GetByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        // Clean phone number for comparison
        var cleanNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");
        
        var drivers = await _context.Drivers.ToListAsync(cancellationToken);
        return drivers.FirstOrDefault(d => d.PhoneNumber.Value == cleanNumber);
    }

    public async Task<IEnumerable<Domain.AggregatesModel.DriverAggregate.Driver>> GetOnlineDriversAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Drivers
            .Where(d => d.Status == DriverStatus.Online)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.AggregatesModel.DriverAggregate.Driver>> GetPendingVerificationDriversAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Drivers
            .Where(d => d.VerificationStatus == VerificationStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public Task<IQueryable<Domain.AggregatesModel.DriverAggregate.Driver>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_context.Drivers.AsQueryable());
    }

    public Domain.AggregatesModel.DriverAggregate.Driver Add(Domain.AggregatesModel.DriverAggregate.Driver driver)
    {
        return _context.Drivers.Add(driver).Entity;
    }

    public void Update(Domain.AggregatesModel.DriverAggregate.Driver driver)
    {
        _context.Drivers.Update(driver);
    }

    public void Remove(Domain.AggregatesModel.DriverAggregate.Driver driver)
    {
        _context.Drivers.Remove(driver);
    }
}
