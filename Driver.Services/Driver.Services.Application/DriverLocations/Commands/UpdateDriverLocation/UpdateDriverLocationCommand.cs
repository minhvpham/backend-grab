using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.DriverLocations.Commands.UpdateDriverLocation;

public record UpdateDriverLocationCommand(
    Guid DriverId,
    double Latitude,
    double Longitude
) : IRequest<Result>;
