using Driver.Services.Domain.AggregatesModel.DriverAggregate;

namespace Driver.Services.Application.Drivers.DTOs;

public record DriverDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string VerificationStatus { get; init; } = string.Empty;
    public string? LicenseNumber { get; init; }
    public string? ProfileImageUrl { get; init; }
    public VehicleInfoDto? VehicleInfo { get; init; }
    public DateTimeOffset? VerifiedAt { get; init; }
    public string? RejectionReason { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}

public record VehicleInfoDto
{
    public string VehicleType { get; init; } = string.Empty;
    public string LicensePlate { get; init; } = string.Empty;
    public string? VehicleBrand { get; init; }
    public string? VehicleModel { get; init; }
    public int? VehicleYear { get; init; }
    public string? VehicleColor { get; init; }
}

public record DriverSummaryDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string VerificationStatus { get; init; } = string.Empty;
    public string? ProfileImageUrl { get; init; }
}
