using Driver.Services.Application.Common.ExternalServices;
using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.Commands.UpdateTripStatus;

public class UpdateTripStatusCommandHandler : IRequestHandler<UpdateTripStatusCommand, Result>
{
    private readonly ITripHistoryRepository _tripRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderServiceClient _orderServiceClient;

    public UpdateTripStatusCommandHandler(
        ITripHistoryRepository tripRepository,
        IDriverRepository driverRepository,
        IUnitOfWork unitOfWork,
        IOrderServiceClient orderServiceClient)
    {
        _tripRepository = tripRepository;
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
        _orderServiceClient = orderServiceClient;
    }

    public async Task<Result> Handle(UpdateTripStatusCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        
        if (trip == null)
        {
            return Result.Failure(
                Error.NotFound("Trip.NotFound", $"Trip with ID '{request.TripId}' not found."));
        }

        try
        {
            string? orderStatusUpdate = null;
            var driver = await _driverRepository.GetByIdAsync(trip.DriverId, cancellationToken);
            if (driver == null)
            {
                return Result.Failure(Error.NotFound("Driver.NotFound", $"Driver with ID '{trip.DriverId}' not found."));
            }

            // Call domain method based on status
            switch (request.Status)
            {
                case TripStatus.Accepted:
                    trip.Accept();
                    orderStatusUpdate = "delivering";
                    driver.MarkAsBusy();
                    break;
                    
                case TripStatus.Rejected:
                    trip.Reject();
                    orderStatusUpdate = "cancelled";
                    driver.MarkAsAvailable();
                    break;
                    
                case TripStatus.PickedUp:
                    trip.MarkAsPickedUp();
                    // Order status remains "delivering"
                    break;
                    
                case TripStatus.InTransit:
                    trip.StartDelivery();
                    // Order status remains "delivering"
                    break;
                    
                default:
                    return Result.Failure(
                        Error.Validation("Trip.InvalidStatus", $"Status transition to {request.Status} not supported via this endpoint"));
            }

            // Call Order.Service if needed
            if (orderStatusUpdate != null)
            {
                var orderResult = await _orderServiceClient.UpdateOrderStatusAsync(trip.OrderId, orderStatusUpdate);
                if (orderResult.IsFailure)
                {
                    // Rollback: don't save trip changes
                    return Result.Failure(Error.Failure("OrderService.UpdateFailed", 
                        $"Failed to update order status: {orderResult.Error.Message}"));
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Validation("Trip.UpdateStatusFailed", ex.Message));
        }
    }
}
