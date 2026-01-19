using Driver.Services.Application.Common.ExternalServices;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.EventHandlers;

/// <summary>
/// Handles the TripAcceptedDomainEvent by updating the order status to "driver_accepted".
/// This provides loose coupling by moving external service calls out of the command handler.
/// </summary>
public class TripAcceptedDomainEventHandler : INotificationHandler<TripAcceptedDomainEvent>
{
    private readonly IOrderServiceClient _orderServiceClient;

    public TripAcceptedDomainEventHandler(IOrderServiceClient orderServiceClient)
    {
        _orderServiceClient = orderServiceClient;
    }

    public async Task Handle(TripAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Update order status asynchronously when trip is accepted
        await _orderServiceClient.UpdateOrderStatusAsync(
            notification.OrderId,
            "driver_accepted",
            notification.DriverId
        );
    }
}