using Driver.Services.Application.Drivers.DTOs;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;

namespace Driver.Services.Application.Drivers.Mappings;

public static class DriverMappings
{
    public static DriverDto ToDto(this Domain.AggregatesModel.DriverAggregate.Driver driver)
    {
        return new DriverDto
        {
            Id = driver.Id,
            FullName = driver.FullName,
            PhoneNumber = driver.PhoneNumber.Value,
            Email = driver.Email,
            Status = driver.Status.ToString(),
            VerificationStatus = driver.VerificationStatus.ToString(),
            LicenseNumber = driver.LicenseNumber,
            ProfileImageUrl = driver.ProfileImageUrl,
            CitizenIdImageUrl = driver.CitizenIdImageUrl,
            DriverLicenseImageUrl = driver.DriverLicenseImageUrl,
            DriverRegistrationImageUrl = driver.DriverRegistrationImageUrl,
            VehicleInfo = driver.VehicleInfo?.ToDto(),
            VerifiedAt = driver.VerifiedAt,
            RejectionReason = driver.RejectionReason,
            CreatedAt = driver.CreatedAt,
            UpdatedAt = driver.UpdatedAt
        };
    }

    public static DriverSummaryDto ToSummaryDto(this Domain.AggregatesModel.DriverAggregate.Driver driver)
    {
        return new DriverSummaryDto
        {
            Id = driver.Id,
            FullName = driver.FullName,
            PhoneNumber = driver.PhoneNumber.Value,
            Status = driver.Status.ToString(),
            VerificationStatus = driver.VerificationStatus.ToString(),
            ProfileImageUrl = driver.ProfileImageUrl,
            CitizenIdImageUrl = driver.CitizenIdImageUrl,
            DriverLicenseImageUrl = driver.DriverLicenseImageUrl,
            DriverRegistrationImageUrl = driver.DriverRegistrationImageUrl
        };
    }

    public static VehicleInfoDto? ToDto(this VehicleInfo? vehicleInfo)
    {
        if (vehicleInfo is null)
            return null;

        return new VehicleInfoDto
        {
            VehicleType = vehicleInfo.VehicleType,
            LicensePlate = vehicleInfo.LicensePlate,
            VehicleBrand = vehicleInfo.VehicleBrand,
            VehicleModel = vehicleInfo.VehicleModel,
            VehicleYear = vehicleInfo.VehicleYear,
            VehicleColor = vehicleInfo.VehicleColor
        };
    }
}
