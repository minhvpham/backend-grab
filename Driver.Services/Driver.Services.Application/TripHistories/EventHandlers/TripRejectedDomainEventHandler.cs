using Driver.Services.Application.Common.ExternalServices;
using Driver.Services.Application.TripHistories.Commands.CreateTrip;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.EventHandlers;

/// <summary>
/// Handles the TripRejectedDomainEvent by attempting to automatically reassign the trip to another available driver.
/// If no eligible drivers are found, updates the order status to "driver_rejected".
/// This enables asynchronous trip reassignment after a driver rejection.
/// </summary>
public class TripRejectedDomainEventHandler : INotificationHandler<TripRejectedDomainEvent>
{
    private readonly IMediator _mediator;
    private readonly ITripHistoryRepository _tripRepository;
    private readonly IOrderServiceClient _orderServiceClient;

    public TripRejectedDomainEventHandler(
        IMediator mediator,
        ITripHistoryRepository tripRepository,
        IOrderServiceClient orderServiceClient)
    {
        _mediator = mediator;
        _tripRepository = tripRepository;
        _orderServiceClient = orderServiceClient;
    }

    public async Task Handle(TripRejectedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Get the rejected trip details to access all information needed for creating a new trip
        var rejectedTrip = await _tripRepository.GetByIdAsync(notification.TripId, cancellationToken);
        if (rejectedTrip == null) return;

        // Attempt to create a new trip (find another driver)
        var createTripCommand = new CreateTripCommand(
            rejectedTrip.OrderId,
            rejectedTrip.PickupAddress,
            rejectedTrip.PickupLatitude,
            rejectedTrip.PickupLongitude,
            rejectedTrip.DeliveryAddress,
            rejectedTrip.DeliveryLatitude,
            rejectedTrip.DeliveryLongitude,
            rejectedTrip.Fare,
            rejectedTrip.CustomerNotes
        );

        var createResult = await _mediator.Send(createTripCommand, cancellationToken);

        // If trip creation failed (no eligible drivers), update order status to "driver_rejected"
        if (createResult.IsFailure)
        {
            await _orderServiceClient.UpdateOrderStatusAsync(
                notification.OrderId,
                "driver_rejected",
                notification.DriverId
            );
        }
    }
}