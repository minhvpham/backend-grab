using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.DriverWallets.Commands.ReturnCash;

public record ReturnCashCommand(
    Guid DriverId,
    decimal Amount,
    string? Reference = null,
    string? Description = null) : IRequest<Result>;
