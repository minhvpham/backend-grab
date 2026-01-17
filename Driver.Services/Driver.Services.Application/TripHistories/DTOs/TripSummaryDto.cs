namespace Driver.Services.Application.TripHistories.DTOs;

public class TripSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderId { get; set; } = default!;
    public string DriverName { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string PickupAddress { get; set; } = default!;
    public string DeliveryAddress { get; set; } = default!;
    public decimal Fare { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public double? DistanceKm { get; set; }
}
