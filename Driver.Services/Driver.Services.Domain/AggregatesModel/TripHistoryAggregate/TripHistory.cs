using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.Exceptions;

namespace Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;

public class TripHistory : Entity<string>, IAggregateRoot
{
    public string DriverId { get; private set; } = string.Empty;
    public string OrderId { get; private set; }
    public TripStatus Status { get; private set; }
    
    // Location details
    public string PickupAddress { get; private set; }
    public double PickupLatitude { get; private set; }
    public double PickupLongitude { get; private set; }
    public string DeliveryAddress { get; private set; }
    public double DeliveryLatitude { get; private set; }
    public double DeliveryLongitude { get; private set; }
    
    // Distance and duration
    public double? DistanceKm { get; private set; }
    public int? DurationMinutes { get; private set; }
    
    // Financial
    public decimal Fare { get; private set; }
    public decimal? CashCollected { get; private set; }
    
    // Timestamps
    public DateTime AssignedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? PickedUpAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    
    // Additional info
    public string? CancellationReason { get; private set; }
    public string? FailureReason { get; private set; }
    public string? CustomerNotes { get; private set; }
    public string? DriverNotes { get; private set; }
    
    // Rating
    public int? CustomerRating { get; private set; }
    public string? CustomerFeedback { get; private set; }

    private TripHistory() { } // For EF Core

    public static TripHistory Create(
        string driverId,
        string orderId,
        string pickupAddress,
        double pickupLat,
        double pickupLng,
        string deliveryAddress,
        double deliveryLat,
        double deliveryLng,
        decimal fare,
        string? customerNotes = null)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new DomainValidationException("Order ID is required");

        if (string.IsNullOrWhiteSpace(pickupAddress))
            throw new DomainValidationException("Pickup address is required");

        if (string.IsNullOrWhiteSpace(deliveryAddress))
            throw new DomainValidationException("Delivery address is required");

        if (fare <= 0)
            throw new DomainValidationException("Fare must be greater than zero");

        var trip = new TripHistory
        {
            Id = Guid.NewGuid().ToString(),
            DriverId = driverId,
            OrderId = orderId,
            Status = TripStatus.Assigned,
            PickupAddress = pickupAddress,
            PickupLatitude = pickupLat,
            PickupLongitude = pickupLng,
            DeliveryAddress = deliveryAddress,
            DeliveryLatitude = deliveryLat,
            DeliveryLongitude = deliveryLng,
            Fare = fare,
            CustomerNotes = customerNotes,
            AssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        trip.AddDomainEvent(new TripAssignedDomainEvent(trip.Id, driverId, orderId));
        return trip;
    }

    public void Accept()
    {
        if (Status != TripStatus.Assigned)
            throw new DomainValidationException($"Cannot accept trip in {Status} status");

        Status = TripStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TripAcceptedDomainEvent(Id, DriverId, OrderId));
    }

    public void MarkAsPickedUp()
    {
        if (Status != TripStatus.Accepted)
            throw new DomainValidationException($"Cannot mark as picked up from {Status} status");

        Status = TripStatus.PickedUp;
        PickedUpAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TripPickedUpDomainEvent(Id, DriverId, OrderId));
    }

    public void StartDelivery()
    {
        if (Status != TripStatus.PickedUp)
            throw new DomainValidationException($"Cannot start delivery from {Status} status");

        Status = TripStatus.InTransit;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TripInTransitDomainEvent(Id, DriverId, OrderId));
    }

    public void CompleteDelivery(decimal? cashCollected = null, string? driverNotes = null)
    {
        if (Status != TripStatus.InTransit)
            throw new DomainValidationException($"Cannot complete delivery from {Status} status");

        Status = TripStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        CashCollected = cashCollected;
        DriverNotes = driverNotes;
        UpdatedAt = DateTime.UtcNow;

        // Calculate duration and distance
        if (AcceptedAt.HasValue && DeliveredAt.HasValue)
        {
            DurationMinutes = (int)(DeliveredAt.Value - AcceptedAt.Value).TotalMinutes;
        }

        AddDomainEvent(new TripDeliveredDomainEvent(Id, DriverId, OrderId, cashCollected));
    }

    public void Cancel(string reason)
    {
        if (Status == TripStatus.Delivered || Status == TripStatus.Failed)
            throw new DomainValidationException($"Cannot cancel trip in {Status} status");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainValidationException("Cancellation reason is required");

        Status = TripStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TripCancelledDomainEvent(Id, DriverId, OrderId, reason));
    }

    public void MarkAsFailed(string reason)
    {
        if (Status == TripStatus.Delivered || Status == TripStatus.Cancelled)
            throw new DomainValidationException($"Cannot mark as failed from {Status} status");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainValidationException("Failure reason is required");

        Status = TripStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TripFailedDomainEvent(Id, DriverId, OrderId, reason));
    }

    public void UpdateDistance(double distanceKm)
    {
        if (distanceKm < 0)
            throw new DomainValidationException("Distance cannot be negative");

        DistanceKm = distanceKm;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddCustomerRating(int rating, string? feedback = null)
    {
        if (Status != TripStatus.Delivered)
            throw new DomainValidationException("Can only rate completed trips");

        if (rating < 1 || rating > 5)
            throw new DomainValidationException("Rating must be between 1 and 5");

        CustomerRating = rating;
        CustomerFeedback = feedback;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TripRatedDomainEvent(Id, DriverId, OrderId, rating));
    }

    public bool IsCompleted() => Status == TripStatus.Delivered;
    public bool IsCancelled() => Status == TripStatus.Cancelled;
    public bool IsFailed() => Status == TripStatus.Failed;
    public bool IsActive() => Status != TripStatus.Delivered && 
                              Status != TripStatus.Cancelled && 
                              Status != TripStatus.Failed;
}
