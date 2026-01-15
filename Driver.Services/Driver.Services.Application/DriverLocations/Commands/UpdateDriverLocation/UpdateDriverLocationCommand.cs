using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.DriverLocations.Commands.UpdateDriverLocation;

public record UpdateDriverLocationCommand(
    string DriverId,
    double Latitude,
    double Longitude
) : IRequest<Result>;
