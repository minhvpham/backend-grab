using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.TripHistories.Commands.CancelTrip;

public record CancelTripCommand(
    Guid TripId,
    string Reason) : IRequest<Result>;
