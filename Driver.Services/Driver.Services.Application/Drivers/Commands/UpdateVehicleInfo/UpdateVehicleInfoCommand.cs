using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.UpdateVehicleInfo;

public record UpdateVehicleInfoCommand(
    Guid DriverId,
    string VehicleType,
    string PlateNumber,
    string Brand,
    string Model,
    string Color,
    int Year
) : IRequest<Result>;
