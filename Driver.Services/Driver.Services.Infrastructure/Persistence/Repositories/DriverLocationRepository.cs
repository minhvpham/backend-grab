using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;
using Microsoft.EntityFrameworkCore;

namespace Driver.Services.Infrastructure.Persistence.Repositories;

public class DriverLocationRepository : IDriverLocationRepository
{
    private readonly DriverServicesDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public DriverLocationRepository(DriverServicesDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<DriverLocation?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverLocations
            .FirstOrDefaultAsync(dl => dl.Id == id, cancellationToken);
    }

    public async Task<DriverLocation?> GetLatestByDriverIdAsync(
        string driverId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverLocations
            .Where(dl => dl.DriverId == driverId)
            .OrderByDescending(dl => dl.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<DriverLocation>> GetLocationHistoryAsync(
        string driverId,
        DateTimeOffset since,
        CancellationToken cancellationToken = default)
    {
        return await _context.DriverLocations
            .Where(dl => dl.DriverId == driverId && dl.Timestamp >= since)
            .OrderByDescending(dl => dl.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DriverLocation>> GetNearbyDriverLocationsAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken cancellationToken = default)
    {
        // Get all locations and filter in-memory using the Haversine formula
        // Note: For production, consider using spatial database extensions (PostGIS)
        var allLocations = await _context.DriverLocations.ToListAsync(cancellationToken);

        return allLocations
            .Where(dl => dl.DistanceTo(latitude, longitude) <= radiusKm)
            .ToList();
    }

    public DriverLocation Add(DriverLocation location)
    {
        return _context.DriverLocations.Add(location).Entity;
    }

    public void Update(DriverLocation location)
    {
        _context.DriverLocations.Update(location);
    }
}
