using Driver.Services.Application.TripHistories.DTOs;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;

namespace Driver.Services.Application.TripHistories.Mappings;

public static class TripHistoryMappings
{
    public static TripHistoryDto ToDto(this TripHistory trip)
    {
        return new TripHistoryDto
        {
            Id = trip.Id,
            DriverId = trip.DriverId,
            OrderId = trip.OrderId,
            Status = trip.Status.ToString(),
            PickupAddress = trip.PickupAddress,
            PickupLatitude = trip.PickupLatitude,
            PickupLongitude = trip.PickupLongitude,
            DeliveryAddress = trip.DeliveryAddress,
            DeliveryLatitude = trip.DeliveryLatitude,
            DeliveryLongitude = trip.DeliveryLongitude,
            DistanceKm = trip.DistanceKm,
            DurationMinutes = trip.DurationMinutes,
            Fare = trip.Fare,
            CashCollected = trip.CashCollected,
            AssignedAt = trip.AssignedAt,
            AcceptedAt = trip.AcceptedAt,
            PickedUpAt = trip.PickedUpAt,
            DeliveredAt = trip.DeliveredAt,
            CancelledAt = trip.CancelledAt,
            CancellationReason = trip.CancellationReason,
            FailureReason = trip.FailureReason,
            CustomerNotes = trip.CustomerNotes,
            DriverNotes = trip.DriverNotes,
            CustomerRating = trip.CustomerRating,
            CustomerFeedback = trip.CustomerFeedback,
            IsCompleted = trip.IsCompleted(),
            IsActive = trip.IsActive()
        };
    }

    public static TripSummaryDto ToSummaryDto(this TripHistory trip)
    {
        return new TripSummaryDto
        {
            Id = trip.Id,
            OrderId = trip.OrderId,
            DriverName = trip.Driver?.FullName ?? "Unknown Driver",
            Status = trip.Status.ToString(),
            PickupAddress = trip.PickupAddress,
            DeliveryAddress = trip.DeliveryAddress,
            Fare = trip.Fare,
            AssignedAt = trip.AssignedAt,
            DeliveredAt = trip.DeliveredAt,
            DistanceKm = trip.DistanceKm
        };
    }
}
