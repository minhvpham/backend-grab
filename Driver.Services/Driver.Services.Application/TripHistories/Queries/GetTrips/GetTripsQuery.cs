using Driver.Services.Application.Common.Models;
using Driver.Services.Application.TripHistories.DTOs;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.Queries.GetTrips;

public record GetTripsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    TripStatus? Status = null,
    string? SearchTerm = null
) : IRequest<Result<PaginatedList<TripSummaryDto>>>;