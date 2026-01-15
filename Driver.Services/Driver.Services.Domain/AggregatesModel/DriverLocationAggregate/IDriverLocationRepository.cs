using Driver.Services.Domain.Abstractions;

namespace Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;

public interface IDriverLocationRepository : IRepository<DriverLocation>
{
    Task<DriverLocation?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<DriverLocation?> GetLatestByDriverIdAsync(string driverId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DriverLocation>> GetLocationHistoryAsync(string driverId, DateTimeOffset since, CancellationToken cancellationToken = default);
    Task<IEnumerable<DriverLocation>> GetNearbyDriverLocationsAsync(double latitude, double longitude, double radiusKm, CancellationToken cancellationToken = default);
    DriverLocation Add(DriverLocation location);
    void Update(DriverLocation location);
}
