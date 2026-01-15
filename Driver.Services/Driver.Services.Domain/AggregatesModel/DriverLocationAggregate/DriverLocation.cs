using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.Exceptions;

namespace Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;

public class DriverLocation : Entity<string>, IAggregateRoot
{
    public string DriverId { get; private set; } = string.Empty;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double? Accuracy { get; private set; } // in meters
    public double? Heading { get; private set; } // compass direction (0-360 degrees)
    public double? Speed { get; private set; } // in meters per second
    public DateTimeOffset Timestamp { get; private set; }

    // EF Core constructor
    private DriverLocation() { }

    // Factory method
    public static DriverLocation Create(
        string driverId,
        double latitude,
        double longitude,
        double? accuracy = null,
        double? heading = null,
        double? speed = null)
    {
        ValidateCoordinates(latitude, longitude);

        var location = new DriverLocation
        {
            Id = Guid.NewGuid().ToString(),
            DriverId = driverId,
            Latitude = latitude,
            Longitude = longitude,
            Accuracy = accuracy,
            Heading = ValidateHeading(heading),
            Speed = ValidateSpeed(speed),
            Timestamp = DateTimeOffset.UtcNow
        };

        return location;
    }

    // Update location
    public void UpdateLocation(
        double latitude,
        double longitude,
        double? accuracy = null,
        double? heading = null,
        double? speed = null)
    {
        ValidateCoordinates(latitude, longitude);

        Latitude = latitude;
        Longitude = longitude;
        Accuracy = accuracy;
        Heading = ValidateHeading(heading);
        Speed = ValidateSpeed(speed);
        Timestamp = DateTimeOffset.UtcNow;
        UpdateUpdatedAt(DateTimeOffset.UtcNow);
    }

    // Calculate distance to another location (Haversine formula)
    public double DistanceTo(double targetLatitude, double targetLongitude)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians(targetLatitude - Latitude);
        var dLon = DegreesToRadians(targetLongitude - Longitude);

        var lat1 = DegreesToRadians(Latitude);
        var lat2 = DegreesToRadians(targetLatitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) *
                Math.Cos(lat1) * Math.Cos(lat2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c; // Distance in kilometers
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    // Validation
    private static void ValidateCoordinates(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new DomainValidationException($"Invalid latitude: {latitude}. Must be between -90 and 90.");

        if (longitude < -180 || longitude > 180)
            throw new DomainValidationException($"Invalid longitude: {longitude}. Must be between -180 and 180.");
    }

    private static double? ValidateHeading(double? heading)
    {
        if (heading.HasValue && (heading.Value < 0 || heading.Value > 360))
            throw new DomainValidationException($"Invalid heading: {heading}. Must be between 0 and 360.");

        return heading;
    }

    private static double? ValidateSpeed(double? speed)
    {
        if (speed.HasValue && speed.Value < 0)
            throw new DomainValidationException($"Invalid speed: {speed}. Must be non-negative.");

        return speed;
    }
}
