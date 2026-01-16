using Driver.Services.Application.DriverLocations.DTOs;
using Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;

namespace Driver.Services.Application.DriverLocations.Mappings;

public static class DriverLocationMappings
{
    public static DriverLocationDto ToDto(this DriverLocation driverLocation)
    {
        return new DriverLocationDto
        {
            DriverId = driverLocation.DriverId,
            Latitude = driverLocation.Latitude,
            Longitude = driverLocation.Longitude,
            LastUpdated = driverLocation.Timestamp
        };
    }

    public static NearbyDriverDto ToNearbyDriverDto(
        this DriverLocation driverLocation, 
        string fullName,
        string? profileImageUrl,
        string status,
        double distanceInKm)
    {
        return new NearbyDriverDto
        {
            DriverId = driverLocation.DriverId,
            FullName = fullName,
            ProfileImageUrl = profileImageUrl,
            Latitude = driverLocation.Latitude,
            Longitude = driverLocation.Longitude,
            DistanceInKm = Math.Round(distanceInKm, 2),
            Status = status
        };
    }
}
