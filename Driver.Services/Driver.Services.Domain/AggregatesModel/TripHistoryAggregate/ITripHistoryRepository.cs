using Driver.Services.Domain.Abstractions;

namespace Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;

public interface ITripHistoryRepository : IRepository<TripHistory>
{
    Task<TripHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TripHistory?> GetByOrderIdAsync(string orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TripHistory>> GetByDriverIdAsync(
        Guid driverId, 
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TripHistory>> GetActiveTripsAsync(
        Guid driverId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TripHistory>> GetCompletedTripsAsync(
        Guid driverId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TripHistory>> GetTripsByStatusAsync(
        TripStatus status,
        CancellationToken cancellationToken = default);
    Task<int> GetTotalTripsCountAsync(
        Guid driverId,
        CancellationToken cancellationToken = default);
    Task<decimal> GetTotalEarningsAsync(
        Guid driverId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
    TripHistory Add(TripHistory trip);
    void Update(TripHistory trip);
    void Remove(TripHistory trip);
}
