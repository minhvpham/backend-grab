using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.UpdateVehicleInfo;

public class UpdateVehicleInfoCommandHandler : IRequestHandler<UpdateVehicleInfoCommand, Result>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleInfoCommandHandler(
        IDriverRepository driverRepository,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateVehicleInfoCommand request, CancellationToken cancellationToken)
    {
        // Check if driver exists
        var driver = await _driverRepository.GetByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure(
                Error.NotFound("Driver.NotFound", $"Driver with ID '{request.DriverId}' not found."));
        }

        // Update vehicle information
        try
        {
            var vehicleInfo = new VehicleInfo(
                request.VehicleType,
                request.PlateNumber,
                request.Brand,
                request.Model,
                request.Year,
                request.Color);

            driver.UpdateVehicleInfo(vehicleInfo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(
                Error.Validation("Driver.UpdateVehicleInfo", ex.Message));
        }
    }
}
