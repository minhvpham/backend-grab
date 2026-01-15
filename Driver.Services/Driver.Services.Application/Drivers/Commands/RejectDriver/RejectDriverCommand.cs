using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.RejectDriver;

public record RejectDriverCommand(
    string DriverId,
    string RejectionReason
) : IRequest<Result>;
