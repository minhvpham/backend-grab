using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.UpdateDriverProfile;

public class UpdateDriverProfileCommandHandler : IRequestHandler<UpdateDriverProfileCommand, Result>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDriverProfileCommandHandler(
        IDriverRepository driverRepository,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateDriverProfileCommand request, CancellationToken cancellationToken)
    {
        // Check if driver exists
        var driver = await _driverRepository.GetByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure(
                Error.NotFound("Driver.NotFound", $"Driver with ID '{request.DriverId}' not found."));
        }

        // Check if email is already taken by another driver
        if (request.Email != driver.Email)
        {
            var existingDriverWithEmail = await _driverRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingDriverWithEmail != null && existingDriverWithEmail.Id != request.DriverId)
            {
                return Result.Failure(
                    Error.Conflict("Driver.EmailConflict", $"A driver with email '{request.Email}' already exists."));
            }
        }

        // Update driver profile
        try
        {
            driver.UpdateProfile(request.FullName, request.Email);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(
                Error.Validation("Driver.UpdateProfile", ex.Message));
        }
    }
}
