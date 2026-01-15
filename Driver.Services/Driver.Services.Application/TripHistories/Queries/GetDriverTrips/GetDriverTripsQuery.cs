using Driver.Services.Application.Common.Models;
using Driver.Services.Application.TripHistories.DTOs;
using MediatR;

namespace Driver.Services.Application.TripHistories.Queries.GetDriverTrips;

public record GetDriverTripsQuery(
    string DriverId,
    bool ActiveOnly = false,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<List<TripSummaryDto>>>;
