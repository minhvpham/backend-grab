using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.EventHandlers;

/// <summary>
/// Handles the TripAssignedDomainEvent by changing the driver's status to WaitingForAcceptance.
/// This ensures that assigned drivers are not available for new trip assignments until they respond.
/// </summary>
public class TripAssignedDomainEventHandler : INotificationHandler<TripAssignedDomainEvent>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TripAssignedDomainEventHandler(
        IDriverRepository driverRepository,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(TripAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Get the assigned driver
        var driver = await _driverRepository.GetByIdAsync(notification.DriverId, cancellationToken);
        if (driver == null) return;

        // Mark driver as waiting for acceptance (changes status from Online to WaitingForAcceptance)
        driver.MarkAsWaitingForAcceptance();

        // Save the status change
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}