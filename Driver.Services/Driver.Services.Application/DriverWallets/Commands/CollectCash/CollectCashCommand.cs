using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.DriverWallets.Commands.CollectCash;

public record CollectCashCommand(
    string DriverId,
    decimal Amount,
    string OrderId,
    string? Reference = null) : IRequest<Result>;
