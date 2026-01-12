using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.TripHistories.Commands.CompleteTrip;

public record CompleteTripCommand(
    Guid TripId,
    decimal? CashCollected = null,
    string? DriverNotes = null) : IRequest<Result>;
