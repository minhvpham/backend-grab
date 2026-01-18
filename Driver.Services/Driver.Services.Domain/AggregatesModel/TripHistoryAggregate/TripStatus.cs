namespace Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;

public enum TripStatus
{
    Assigned = 1,        // Order assigned to driver
    Accepted = 2,        // Driver accepted the order
    PickedUp = 3,        // Driver picked up the order
    InTransit = 4,       // On the way to customer
    Delivered = 5,       // Successfully delivered
    Cancelled = 6,       // Trip was cancelled
    Failed = 7,          // Delivery failed
    Rejected = 8         // Driver rejected assignment
}
