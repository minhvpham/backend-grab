using Driver.Services.Application.Common.Models;
using Driver.Services.Application.TripHistories.DTOs;
using Driver.Services.Application.TripHistories.Mappings;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.Queries.GetDriverTrips;

public class GetDriverTripsQueryHandler : IRequestHandler<GetDriverTripsQuery, Result<List<TripSummaryDto>>>
{
    private readonly ITripHistoryRepository _tripRepository;

    public GetDriverTripsQueryHandler(ITripHistoryRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<Result<List<TripSummaryDto>>> Handle(
        GetDriverTripsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<TripHistory> trips;

        if (request.ActiveOnly)
        {
            trips = await _tripRepository.GetActiveTripsAsync(request.DriverId, cancellationToken);
        }
        else
        {
            trips = await _tripRepository.GetByDriverIdAsync(request.DriverId, cancellationToken);
        }

        var tripSummaries = trips
            .OrderByDescending(t => t.AssignedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => t.ToSummaryDto())
            .ToList();

        return Result.Success(tripSummaries);
    }
}
