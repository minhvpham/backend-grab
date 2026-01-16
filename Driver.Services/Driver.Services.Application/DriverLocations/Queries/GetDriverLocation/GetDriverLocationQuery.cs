using Driver.Services.Application.Common.Models;
using Driver.Services.Application.DriverLocations.DTOs;
using MediatR;

namespace Driver.Services.Application.DriverLocations.Queries.GetDriverLocation;

public record GetDriverLocationQuery(string DriverId) : IRequest<Result<DriverLocationDto>>;
