using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.Commands.UpdateTripStatus;

public record UpdateTripStatusCommand(
    string TripId,
    TripStatus Status) : IRequest<Result>;
