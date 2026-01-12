using Driver.Services.Domain.Abstractions;

namespace Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;

public record TripAssignedDomainEvent(
    Guid TripId,
    Guid DriverId,
    string OrderId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripAcceptedDomainEvent(
    Guid TripId,
    Guid DriverId,
    string OrderId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripPickedUpDomainEvent(
    Guid TripId,
    Guid DriverId,
    string OrderId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripInTransitDomainEvent(
    Guid TripId,
    Guid DriverId,
    string OrderId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripDeliveredDomainEvent(
    Guid TripId,
    Guid DriverId,
    string OrderId,
    decimal? CashCollected) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripCancelledDomainEvent(
    Guid TripId,
    Guid DriverId,
    string OrderId,
    string Reason) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripFailedDomainEvent(
    Guid TripId,
    Guid DriverId,
    string OrderId,
    string Reason) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripRatedDomainEvent(
    Guid TripId,
    Guid DriverId,
    string OrderId,
    int Rating) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
