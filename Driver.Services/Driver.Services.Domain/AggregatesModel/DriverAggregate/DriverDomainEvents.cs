using Driver.Services.Domain.Abstractions;

namespace Driver.Services.Domain.AggregatesModel.DriverAggregate;

public class DriverRegisteredDomainEvent : IDomainEvent
{
    public string DriverId { get; }
    public string FullName { get; }
    public string Email { get; }
    public DateTimeOffset OccurredOn { get; }

    public DriverRegisteredDomainEvent(string driverId, string fullName, string email)
    {
        DriverId = driverId;
        FullName = fullName;
        Email = email;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}

public class DriverVerifiedDomainEvent : IDomainEvent
{
    public string DriverId { get; }
    public string FullName { get; }
    public DateTimeOffset OccurredOn { get; }

    public DriverVerifiedDomainEvent(string driverId, string fullName)
    {
        DriverId = driverId;
        FullName = fullName;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}

public class DriverRejectedDomainEvent : IDomainEvent
{
    public string DriverId { get; }
    public string FullName { get; }
    public string Reason { get; }
    public DateTimeOffset OccurredOn { get; }

    public DriverRejectedDomainEvent(string driverId, string fullName, string reason)
    {
        DriverId = driverId;
        FullName = fullName;
        Reason = reason;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}

public class DriverVehicleUpdatedDomainEvent : IDomainEvent
{
    public string DriverId { get; }
    public string VehicleType { get; }
    public string LicensePlate { get; }
    public DateTimeOffset OccurredOn { get; }

    public DriverVehicleUpdatedDomainEvent(string driverId, string vehicleType, string licensePlate)
    {
        DriverId = driverId;
        VehicleType = vehicleType;
        LicensePlate = licensePlate;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}

public class DriverDeletedDomainEvent : IDomainEvent
{
    public string DriverId { get; }
    public string FullName { get; }
    public DateTimeOffset OccurredOn { get; }

    public DriverDeletedDomainEvent(string driverId, string fullName)
    {
        DriverId = driverId;
        FullName = fullName;
        OccurredOn = DateTimeOffset.UtcNow;
    }
}
