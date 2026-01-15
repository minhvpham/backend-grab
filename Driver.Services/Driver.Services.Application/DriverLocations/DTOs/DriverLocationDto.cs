namespace Driver.Services.Application.DriverLocations.DTOs;

public class DriverLocationDto
{
    public string DriverId { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTimeOffset LastUpdated { get; set; }
}

public class NearbyDriverDto
{
    public string DriverId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double DistanceInKm { get; set; }
    public string Status { get; set; } = string.Empty;
}
