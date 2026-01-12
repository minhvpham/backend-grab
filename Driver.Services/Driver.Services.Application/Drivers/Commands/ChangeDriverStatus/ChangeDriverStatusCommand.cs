using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.ChangeDriverStatus;

public record ChangeDriverStatusCommand(
    Guid DriverId,
    DriverStatus NewStatus
) : IRequest<Result>;
