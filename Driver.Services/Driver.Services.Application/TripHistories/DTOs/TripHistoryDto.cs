namespace Driver.Services.Application.TripHistories.DTOs;

public class TripHistoryDto
{
    public string Id { get; set; } = string.Empty;
    public string DriverId { get; set; } = string.Empty;
    public string OrderId { get; set; } = default!;
    public string Status { get; set; } = default!;
    
    // Locations
    public string PickupAddress { get; set; } = default!;
    public double PickupLatitude { get; set; }
    public double PickupLongitude { get; set; }
    public string DeliveryAddress { get; set; } = default!;
    public double DeliveryLatitude { get; set; }
    public double DeliveryLongitude { get; set; }
    
    // Trip metrics
    public double? DistanceKm { get; set; }
    public int? DurationMinutes { get; set; }
    
    // Financial
    public decimal Fare { get; set; }
    public decimal? CashCollected { get; set; }
    
    // Timestamps
    public DateTime AssignedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Additional info
    public string? CancellationReason { get; set; }
    public string? FailureReason { get; set; }
    public string? CustomerNotes { get; set; }
    public string? DriverNotes { get; set; }
    
    // Rating
    public int? CustomerRating { get; set; }
    public string? CustomerFeedback { get; set; }
    
    public bool IsCompleted { get; set; }
    public bool IsActive { get; set; }
}
