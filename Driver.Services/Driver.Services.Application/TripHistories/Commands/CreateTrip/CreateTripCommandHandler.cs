using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.Commands.CreateTrip;

public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, Result<string>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly ITripHistoryRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTripCommandHandler(
        IDriverRepository driverRepository,
        ITripHistoryRepository tripRepository,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        // Verify driver exists
        var driver = await _driverRepository.GetByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure<string>(
                Error.NotFound("Driver.NotFound", $"Driver with ID '{request.DriverId}' not found."));
        }

        // Check if order already has a trip
        var existingTrip = await _tripRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (existingTrip != null)
        {
            return Result.Failure<string>(
                Error.Conflict("Trip.OrderExists", $"Order '{request.OrderId}' already has a trip."));
        }

        try
        {
            var trip = TripHistory.Create(
                request.DriverId,
                request.OrderId,
                request.PickupAddress,
                request.PickupLatitude,
                request.PickupLongitude,
                request.DeliveryAddress,
                request.DeliveryLatitude,
                request.DeliveryLongitude,
                request.Fare,
                request.CustomerNotes);

            _tripRepository.Add(trip);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(trip.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(
                Error.Validation("Trip.CreateFailed", ex.Message));
        }
    }
}
