using Driver.Services.Domain.Exceptions;

namespace Driver.Services.Domain.AggregatesModel.DriverAggregate;

public class VehicleInfo
{
    public string VehicleType { get; private set; }
    public string LicensePlate { get; private set; }
    public string? VehicleBrand { get; private set; }
    public string? VehicleModel { get; private set; }
    public int? VehicleYear { get; private set; }
    public string? VehicleColor { get; private set; }

    private VehicleInfo() { } // EF Core constructor

    public VehicleInfo(
        string vehicleType,
        string licensePlate,
        string? vehicleBrand = null,
        string? vehicleModel = null,
        int? vehicleYear = null,
        string? vehicleColor = null)
    {
        if (string.IsNullOrWhiteSpace(vehicleType))
            throw new DomainValidationException("Vehicle type cannot be empty");

        if (string.IsNullOrWhiteSpace(licensePlate))
            throw new DomainValidationException("License plate cannot be empty");

        if (vehicleYear.HasValue && (vehicleYear.Value < 1900 || vehicleYear.Value > DateTimeOffset.UtcNow.Year + 1))
            throw new DomainValidationException($"Vehicle year must be between 1900 and {DateTimeOffset.UtcNow.Year + 1}");

        VehicleType = vehicleType.Trim();
        LicensePlate = licensePlate.Trim().ToUpperInvariant();
        VehicleBrand = vehicleBrand?.Trim();
        VehicleModel = vehicleModel?.Trim();
        VehicleYear = vehicleYear;
        VehicleColor = vehicleColor?.Trim();
    }

    public VehicleInfo UpdateVehicleInfo(
        string? vehicleType = null,
        string? licensePlate = null,
        string? vehicleBrand = null,
        string? vehicleModel = null,
        int? vehicleYear = null,
        string? vehicleColor = null)
    {
        return new VehicleInfo(
            vehicleType ?? VehicleType,
            licensePlate ?? LicensePlate,
            vehicleBrand ?? VehicleBrand,
            vehicleModel ?? VehicleModel,
            vehicleYear ?? VehicleYear,
            vehicleColor ?? VehicleColor
        );
    }
}
