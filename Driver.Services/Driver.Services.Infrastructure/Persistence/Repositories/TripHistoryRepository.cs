using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using Microsoft.EntityFrameworkCore;

namespace Driver.Services.Infrastructure.Persistence.Repositories;

public class TripHistoryRepository : ITripHistoryRepository
{
    private readonly DriverServicesDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public TripHistoryRepository(DriverServicesDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<TripHistory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.TripHistories
            .FirstOrDefaultAsync(th => th.Id == id, cancellationToken);
    }

    public async Task<TripHistory?> GetByOrderIdAsync(
        string orderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TripHistories
            .FirstOrDefaultAsync(th => th.OrderId == orderId, cancellationToken);
    }

    public async Task<IReadOnlyList<TripHistory>> GetByDriverIdAsync(
        Guid driverId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TripHistories
            .Where(th => th.DriverId == driverId)
            .OrderByDescending(th => th.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TripHistory>> GetActiveTripsAsync(
        Guid driverId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TripHistories
            .Where(th => th.DriverId == driverId && 
                         th.Status != TripStatus.Delivered &&
                         th.Status != TripStatus.Cancelled &&
                         th.Status != TripStatus.Failed)
            .OrderByDescending(th => th.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TripHistory>> GetCompletedTripsAsync(
        Guid driverId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TripHistories
            .Where(th => th.DriverId == driverId && th.Status == TripStatus.Delivered);

        if (from.HasValue)
            query = query.Where(th => th.DeliveredAt >= from.Value);

        if (to.HasValue)
            query = query.Where(th => th.DeliveredAt <= to.Value);

        return await query
            .OrderByDescending(th => th.DeliveredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TripHistory>> GetTripsByStatusAsync(
        TripStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.TripHistories
            .Where(th => th.Status == status)
            .OrderByDescending(th => th.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalTripsCountAsync(
        Guid driverId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TripHistories
            .CountAsync(th => th.DriverId == driverId, cancellationToken);
    }

    public async Task<decimal> GetTotalEarningsAsync(
        Guid driverId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TripHistories
            .Where(th => th.DriverId == driverId && th.Status == TripStatus.Delivered);

        if (from.HasValue)
            query = query.Where(th => th.DeliveredAt >= from.Value);

        if (to.HasValue)
            query = query.Where(th => th.DeliveredAt <= to.Value);

        return await query.SumAsync(th => th.Fare, cancellationToken);
    }

    public TripHistory Add(TripHistory trip)
    {
        return _context.TripHistories.Add(trip).Entity;
    }

    public void Update(TripHistory trip)
    {
        _context.TripHistories.Update(trip);
    }

    public void Remove(TripHistory trip)
    {
        _context.TripHistories.Remove(trip);
    }
}
