using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;
using MediatR;

namespace Driver.Services.Application.DriverLocations.Commands.UpdateDriverLocation;

public class UpdateDriverLocationCommandHandler : IRequestHandler<UpdateDriverLocationCommand, Result>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverLocationRepository _driverLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDriverLocationCommandHandler(
        IDriverRepository driverRepository,
        IDriverLocationRepository driverLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _driverLocationRepository = driverLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateDriverLocationCommand request, CancellationToken cancellationToken)
    {
        // Verify driver exists
        var driver = await _driverRepository.GetByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure(
                Error.NotFound("Driver.NotFound", $"Driver with ID '{request.DriverId}' not found."));
        }

        try
        {
            // Get or create driver location
            var driverLocation = await _driverLocationRepository.GetLatestByDriverIdAsync(request.DriverId, cancellationToken);
            
            if (driverLocation == null)
            {
                // Create new location
                driverLocation = DriverLocation.Create(request.DriverId, request.Latitude, request.Longitude);
                _driverLocationRepository.Add(driverLocation);
            }
            else
            {
                // Update existing location
                driverLocation.UpdateLocation(request.Latitude, request.Longitude);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(
                Error.Validation("DriverLocation.ValidationFailed", ex.Message));
        }
    }
}
