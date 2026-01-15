using Driver.Services.Application.Common.Models;
using Driver.Services.Application.Drivers.DTOs;
using Driver.Services.Application.Drivers.Mappings;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.Exceptions;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.RegisterDriver;

public class RegisterDriverCommandHandler : IRequestHandler<RegisterDriverCommand, Result<DriverDto>>
{
    private readonly IDriverRepository _driverRepository;

    public RegisterDriverCommandHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<Result<DriverDto>> Handle(
        RegisterDriverCommand request,
        CancellationToken cancellationToken)
    {
        // Check if provided driver ID already exists
        string? driverId = null;
        if (!string.IsNullOrEmpty(request.DriverId))
        {
            driverId = request.DriverId;

            var existingDriverById = await _driverRepository.GetByIdAsync(driverId, cancellationToken);
            if (existingDriverById is not null)
            {
                return Result.Failure<DriverDto>(
                    Error.Conflict("Driver.IdExists", $"Driver with ID '{driverId}' already exists"));
            }
        }

        // Check if driver with same email already exists
        var existingDriverByEmail = await _driverRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingDriverByEmail is not null)
        {
            return Result.Failure<DriverDto>(
                Error.Conflict("Driver.EmailExists", $"Driver with email '{request.Email}' already exists"));
        }

        // Check if driver with same phone number already exists
        var existingDriverByPhone = await _driverRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (existingDriverByPhone is not null)
        {
            return Result.Failure<DriverDto>(
                Error.Conflict("Driver.PhoneExists", $"Driver with phone number '{request.PhoneNumber}' already exists"));
        }

        try
        {
            // Create new driver
            var driver = Domain.AggregatesModel.DriverAggregate.Driver.Create(
                request.FullName,
                request.PhoneNumber,
                request.Email,
                request.LicenseNumber,
                driverId);

            _driverRepository.Add(driver);

            return Result.Success(driver.ToDto());
        }
        catch (DomainValidationException ex)
        {
            return Result.Failure<DriverDto>(
                Error.Validation("Driver.ValidationFailed", ex.Message));
        }
    }
}
