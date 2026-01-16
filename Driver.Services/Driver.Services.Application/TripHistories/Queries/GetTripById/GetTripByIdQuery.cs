using Driver.Services.Application.Common.Models;
using Driver.Services.Application.TripHistories.DTOs;
using MediatR;

namespace Driver.Services.Application.TripHistories.Queries.GetTripById;

public record GetTripByIdQuery(string TripId) : IRequest<Result<TripHistoryDto>>;
