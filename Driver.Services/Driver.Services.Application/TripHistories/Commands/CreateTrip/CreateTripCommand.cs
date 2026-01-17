using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.TripHistories.Commands.CreateTrip;

public record CreateTripCommand(
    // string DriverId,
    string OrderId,
    string PickupAddress,
    double PickupLatitude,
    double PickupLongitude,
    string DeliveryAddress,
    double DeliveryLatitude,
    double DeliveryLongitude,
    decimal Fare,
    string? CustomerNotes = null
) : IRequest<Result<string>>;
