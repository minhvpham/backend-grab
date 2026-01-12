using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.DriverWallets.Commands.AddFunds;

public record AddFundsCommand(
    Guid DriverId,
    decimal Amount,
    string? Reference = null,
    string? Description = null) : IRequest<Result>;
