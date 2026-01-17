using Driver.Services.Application.Common.ExternalServices;
using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.Commands.CompleteTrip;

public class CompleteTripCommandHandler : IRequestHandler<CompleteTripCommand, Result>
{
    private readonly ITripHistoryRepository _tripRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderServiceClient _orderServiceClient;

    public CompleteTripCommandHandler(
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

    public async Task<Result> Handle(CompleteTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        
        if (trip == null)
        {
            return Result.Failure(
                Error.NotFound("Trip.NotFound", $"Trip with ID '{request.TripId}' not found."));
        }

        var driver = await _driverRepository.GetByIdAsync(trip.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure(Error.NotFound("Driver.NotFound", $"Driver with ID '{trip.DriverId}' not found."));
        }

        try
        {
            trip.CompleteDelivery(request.CashCollected, request.DriverNotes);
            
            // Call Order.Service to update status
            var orderResult = await _orderServiceClient.UpdateOrderStatusAsync(trip.OrderId, "delivered");
            if (orderResult.IsFailure)
            {
                // Rollback: don't save trip changes
                return Result.Failure(Error.Failure("OrderService.UpdateFailed", 
                    $"Failed to update order status: {orderResult.Error.Message}"));
            }
            
            driver.MarkAsAvailable();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Validation("Trip.CompleteFailed", ex.Message));
        }
    }
}
