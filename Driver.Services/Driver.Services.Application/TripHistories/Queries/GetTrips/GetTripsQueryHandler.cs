using Driver.Services.Application.Common.Models;
using Driver.Services.Application.TripHistories.DTOs;
using Driver.Services.Application.TripHistories.Mappings;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.Queries.GetTrips;

public class GetTripsQueryHandler : IRequestHandler<GetTripsQuery, Result<PaginatedList<TripSummaryDto>>>
{
    private readonly ITripHistoryRepository _tripRepository;

    public GetTripsQueryHandler(ITripHistoryRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<Result<PaginatedList<TripSummaryDto>>> Handle(GetTripsQuery request, CancellationToken cancellationToken)
    {
        // Get all trips as queryable
        var tripsQuery = await _tripRepository.GetAllAsync(cancellationToken);

        // Filter by driver ID first
        tripsQuery = tripsQuery.Where(th => th.DriverId == request.DriverId);

        if (request.Status.HasValue)
        {
            tripsQuery = tripsQuery.Where(th => th.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            tripsQuery = tripsQuery.Where(th =>
                th.CustomerNotes != null && th.CustomerNotes.ToLower().Contains(searchTerm));
        }

        // Order by assigned date descending
        tripsQuery = tripsQuery.OrderByDescending(th => th.AssignedAt);

        // Convert to DTO and create paginated result
        var tripDtos = tripsQuery.Select(th => th.ToSummaryDto());
        var paginatedResult = await PaginatedList<TripSummaryDto>.CreateAsync(
            tripDtos,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result.Success(paginatedResult);
    }
}