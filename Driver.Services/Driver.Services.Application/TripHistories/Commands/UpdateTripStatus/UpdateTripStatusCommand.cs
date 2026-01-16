using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.TripHistories.Commands.UpdateTripStatus;

public record UpdateTripStatusCommand(
    string TripId,
    string Action) : IRequest<Result>; // Action: "accept", "pickup", "start_delivery"
