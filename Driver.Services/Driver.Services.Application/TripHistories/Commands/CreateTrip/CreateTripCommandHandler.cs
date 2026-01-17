using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Driver.Services.Application.TripHistories.Commands.CreateTrip;

public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, Result<string>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly ITripHistoryRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTripCommandHandler(
        IDriverRepository driverRepository,
        ITripHistoryRepository tripRepository,
        IUnitOfWork unitOfWork
    )
    {
        _driverRepository = driverRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(
        CreateTripCommand request,
        CancellationToken cancellationToken
    )
    {
        // Check if order has any rejected trip history
        var tripQuery = await _tripRepository.GetAllAsync(cancellationToken);

        // Choose any online driver who hasn't rejected this order
        var onlineDrivers = await _driverRepository.GetOnlineDriversAsync(cancellationToken);
        if (!onlineDrivers.Any())
        {
            return Result.Failure<string>(
                Error.NotFound("Driver.NoAvailable", "No online drivers found.")
            );
        }

        Driver.Services.Domain.AggregatesModel.DriverAggregate.Driver? selectedDriver =
            onlineDrivers
                .Where(d =>
                    !tripQuery.Any(t =>
                        t.DriverId == d.Id
                        && t.OrderId == request.OrderId
                        && t.Status == TripStatus.Rejected
                    )
                )
                .FirstOrDefault();
        if (selectedDriver == null)
        {
            return Result.Failure<string>(
                Error.NotFound(
                    "Driver.NoEligible",
                    "No eligible drivers found (all have rejected this order)."
                )
            );
        }

        try
        {
            var trip = TripHistory.Create(
                selectedDriver.Id,
                request.OrderId,
                request.PickupAddress,
                request.PickupLatitude,
                request.PickupLongitude,
                request.DeliveryAddress,
                request.DeliveryLatitude,
                request.DeliveryLongitude,
                request.Fare,
                request.CustomerNotes
            );

            _tripRepository.Add(trip);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(trip.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(Error.Validation("Trip.CreateFailed", ex.Message));
        }
    }
}
