using Driver.Services.Application.Common.Models;
using Driver.Services.Application.DriverLocations.DTOs;
using Driver.Services.Application.DriverLocations.Mappings;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;
using MediatR;

namespace Driver.Services.Application.DriverLocations.Queries.GetNearbyDrivers;

public class GetNearbyDriversQueryHandler : IRequestHandler<GetNearbyDriversQuery, Result<List<NearbyDriverDto>>>
{
    private readonly IDriverLocationRepository _driverLocationRepository;
    private readonly IDriverRepository _driverRepository;

    public GetNearbyDriversQueryHandler(
        IDriverLocationRepository driverLocationRepository,
        IDriverRepository driverRepository)
    {
        _driverLocationRepository = driverLocationRepository;
        _driverRepository = driverRepository;
    }

    public async Task<Result<List<NearbyDriverDto>>> Handle(
        GetNearbyDriversQuery request,
        CancellationToken cancellationToken)
    {
        // Get nearby driver locations
        var nearbyLocations = await _driverLocationRepository.GetNearbyDriverLocationsAsync(
            request.Latitude,
            request.Longitude,
            request.RadiusInKm,
            cancellationToken);

        // Get driver details for each location
        var nearbyDrivers = new List<NearbyDriverDto>();
        
        foreach (var location in nearbyLocations.Take(request.MaxResults))
        {
            var driver = await _driverRepository.GetByIdAsync(location.DriverId, cancellationToken);
            if (driver != null && driver.Status == DriverStatus.Online)
            {
                var distance = location.DistanceTo(
                    request.Latitude,
                    request.Longitude);

                var nearbyDriver = location.ToNearbyDriverDto(
                    driver.FullName,
                    driver.ProfileImageUrl,
                    driver.Status.ToString(),
                    distance);

                nearbyDrivers.Add(nearbyDriver);
            }
        }

        return Result.Success(nearbyDrivers);
    }
}
