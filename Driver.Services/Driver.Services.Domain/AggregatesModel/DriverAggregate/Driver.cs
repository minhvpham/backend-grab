using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.Exceptions;

namespace Driver.Services.Domain.AggregatesModel.DriverAggregate;

public class Driver : Entity<string>, IAggregateRoot
{
    public string FullName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public DriverStatus Status { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; }
    public VehicleInfo? VehicleInfo { get; private set; }
    public string? LicenseNumber { get; private set; }
    public string? ProfileImageUrl { get; private set; }
    public string? CitizenIdImageUrl { get; private set; }
    public string? DriverLicenseImageUrl { get; private set; }
    public string? DriverRegistrationImageUrl { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    // EF Core constructor
    private Driver() { }

    // Factory method for creating new driver
    public static Driver Create(
        string fullName,
        string phoneNumber,
        string email,
        string? licenseNumber = null,
        string? citizenIdImageUrl = null,
        string? driverLicenseImageUrl = null,
        string? driverRegistrationImageUrl = null,
        string? id = null)
    {
        var driver = new Driver
        {
            Id = id ?? Guid.NewGuid().ToString(),
            FullName = ValidateFullName(fullName),
            PhoneNumber = new PhoneNumber(phoneNumber),
            Email = ValidateEmail(email),
            Status = DriverStatus.Offline,
            VerificationStatus = VerificationStatus.Pending,
            LicenseNumber = licenseNumber?.Trim(),
            CitizenIdImageUrl = citizenIdImageUrl?.Trim(),
            DriverLicenseImageUrl = driverLicenseImageUrl?.Trim(),
            DriverRegistrationImageUrl = driverRegistrationImageUrl?.Trim()
        };

        driver.AddDomainEvent(new DriverRegisteredDomainEvent(driver.Id, driver.FullName, driver.Email));
        
        return driver;
    }

    // Status management
    public void GoOnline()
    {
        if (VerificationStatus != VerificationStatus.Verified)
            throw new DomainValidationException("Driver must be verified before going online");

        if (VehicleInfo == null)
            throw new DomainValidationException("Driver must have vehicle information before going online");

        Status = DriverStatus.Online;
        UpdateUpdatedAt(DateTimeOffset.UtcNow);
    }

    public void GoOffline()
    {
        if (Status == DriverStatus.Busy)
            throw new DomainValidationException("Cannot go offline while busy with an order");

        Status = DriverStatus.Offline;
        UpdateUpdatedAt(DateTimeOffset.UtcNow);
    }

    public void MarkAsBusy()
    {
        if (Status != DriverStatus.Online)
            throw new DomainValidationException("Driver must be online to be marked as busy");

        Status = DriverStatus.Busy;
        UpdateUpdatedAt(DateTimeOffset.UtcNow);
    }

    public void MarkAsAvailable()
    {
        if (Status != DriverStatus.Busy)
            throw new DomainValidationException("Driver must be busy to be marked as available");

        Status = DriverStatus.Online;
        UpdateUpdatedAt(DateTimeOffset.UtcNow);
    }

    // Verification
    public void Verify()
    {
        if (VerificationStatus == VerificationStatus.Verified)
            throw new DomainValidationException("Driver is already verified");

        VerificationStatus = VerificationStatus.Verified;
        VerifiedAt = DateTimeOffset.UtcNow;
        RejectionReason = null;
        UpdateUpdatedAt(DateTimeOffset.UtcNow);

        AddDomainEvent(new DriverVerifiedDomainEvent(Id, FullName));
    }

    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainValidationException("Rejection reason cannot be empty");

        VerificationStatus = VerificationStatus.Rejected;
        RejectionReason = reason.Trim();
        UpdateUpdatedAt(DateTimeOffset.UtcNow);

        AddDomainEvent(new DriverRejectedDomainEvent(Id, FullName, reason));
    }

    // Vehicle management
    public void UpdateVehicleInfo(VehicleInfo vehicleInfo)
    {
        VehicleInfo = vehicleInfo ?? throw new DomainValidationException("Vehicle info cannot be null");
        UpdateUpdatedAt(DateTimeOffset.UtcNow);

        AddDomainEvent(new DriverVehicleUpdatedDomainEvent(Id, vehicleInfo.VehicleType, vehicleInfo.LicensePlate));
    }

    // Profile management
    public void UpdateProfile(string? fullName = null, string? email = null, string? phoneNumber = null, string? profileImageUrl = null,
        string? citizenIdImageUrl = null, string? driverLicenseImageUrl = null, string? driverRegistrationImageUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
            FullName = ValidateFullName(fullName);

        if (!string.IsNullOrWhiteSpace(email))
            Email = ValidateEmail(email);

        if (!string.IsNullOrWhiteSpace(phoneNumber))
            PhoneNumber = new PhoneNumber(phoneNumber);

        if (!string.IsNullOrWhiteSpace(profileImageUrl))
            ProfileImageUrl = profileImageUrl.Trim();

        if (citizenIdImageUrl != null)
            CitizenIdImageUrl = citizenIdImageUrl.Trim();

        if (driverLicenseImageUrl != null)
            DriverLicenseImageUrl = driverLicenseImageUrl.Trim();

        if (driverRegistrationImageUrl != null)
            DriverRegistrationImageUrl = driverRegistrationImageUrl.Trim();

        UpdateUpdatedAt(DateTimeOffset.UtcNow);
    }

    // Validation helpers
    private static string ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainValidationException("Full name cannot be empty");

        if (fullName.Length < 2 || fullName.Length > 100)
            throw new DomainValidationException("Full name must be between 2 and 100 characters");

        return fullName.Trim();
    }

    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainValidationException("Email cannot be empty");

        // Basic email validation
        if (!email.Contains("@") || !email.Contains("."))
            throw new DomainValidationException("Invalid email format");

        return email.Trim().ToLowerInvariant();
    }
}
