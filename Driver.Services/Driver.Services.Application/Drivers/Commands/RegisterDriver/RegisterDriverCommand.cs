using Driver.Services.Application.Common.Models;
using Driver.Services.Application.Drivers.DTOs;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.RegisterDriver;

public record RegisterDriverCommand(
    string FullName,
    string PhoneNumber,
    string Email,
    string LicenseNumber,
    Guid? DriverId = null) : IRequest<Result<DriverDto>>;
