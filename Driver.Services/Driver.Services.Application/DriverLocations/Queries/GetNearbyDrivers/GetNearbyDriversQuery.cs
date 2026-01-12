using Driver.Services.Application.Common.Models;
using Driver.Services.Application.DriverLocations.DTOs;
using MediatR;

namespace Driver.Services.Application.DriverLocations.Queries.GetNearbyDrivers;

public record GetNearbyDriversQuery(
    double Latitude,
    double Longitude,
    double RadiusInKm = 5.0,
    int MaxResults = 10
) : IRequest<Result<List<NearbyDriverDto>>>;
