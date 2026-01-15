using Driver.Services.Application.Common.Models;
using Driver.Services.Application.DriverWallets.DTOs;
using MediatR;

namespace Driver.Services.Application.DriverWallets.Queries.GetWalletBalance;

public record GetWalletBalanceQuery(string DriverId) : IRequest<Result<WalletBalanceDto>>;
