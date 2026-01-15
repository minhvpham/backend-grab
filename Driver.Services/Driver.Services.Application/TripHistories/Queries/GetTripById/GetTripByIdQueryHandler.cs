using Driver.Services.Application.Common.Models;
using Driver.Services.Application.TripHistories.DTOs;
using Driver.Services.Application.TripHistories.Mappings;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.Queries.GetTripById;

public class GetTripByIdQueryHandler : IRequestHandler<GetTripByIdQuery, Result<TripHistoryDto>>
{
    private readonly ITripHistoryRepository _tripRepository;

    public GetTripByIdQueryHandler(ITripHistoryRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<Result<TripHistoryDto>> Handle(
        GetTripByIdQuery request,
        CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);

        if (trip == null)
        {
            return Result.Failure<TripHistoryDto>(
                Error.NotFound("Trip.NotFound", $"Trip with ID '{request.TripId}' not found."));
        }

        return Result.Success(trip.ToDto());
    }
}
