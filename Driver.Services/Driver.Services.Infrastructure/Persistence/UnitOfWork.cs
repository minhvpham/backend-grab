using Driver.Services.Domain.Abstractions;

namespace Driver.Services.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DriverServicesDbContext _context;

    public UnitOfWork(DriverServicesDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Dispatch domain events before saving
            // (In a real application, you'd implement a domain event dispatcher here)
            
            var result = await _context.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        catch (Exception)
        {
            // Log exception if needed
            return false;
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
