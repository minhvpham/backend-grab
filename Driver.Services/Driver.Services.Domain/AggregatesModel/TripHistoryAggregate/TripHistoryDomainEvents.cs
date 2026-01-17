using Driver.Services.Domain.Abstractions;

namespace Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;

public record TripAssignedDomainEvent(
    string TripId,
    string DriverId,
    string OrderId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripAcceptedDomainEvent(
    string TripId,
    string DriverId,
    string OrderId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripRejectedDomainEvent(
    string TripId,
    string DriverId,
    string OrderId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripPickedUpDomainEvent(
    string TripId,
    string DriverId,
    string OrderId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripInTransitDomainEvent(
    string TripId,
    string DriverId,
    string OrderId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripDeliveredDomainEvent(
    string TripId,
    string DriverId,
    string OrderId,
    decimal? CashCollected) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripCancelledDomainEvent(
    string TripId,
    string DriverId,
    string OrderId,
    string Reason) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripFailedDomainEvent(
    string TripId,
    string DriverId,
    string OrderId,
    string Reason) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public record TripRatedDomainEvent(
    string TripId,
    string DriverId,
    string OrderId,
    int Rating) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
