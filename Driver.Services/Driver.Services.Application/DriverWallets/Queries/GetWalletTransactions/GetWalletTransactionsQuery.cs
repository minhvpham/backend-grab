using Driver.Services.Application.Common.Models;
using Driver.Services.Application.DriverWallets.DTOs;
using MediatR;

namespace Driver.Services.Application.DriverWallets.Queries.GetWalletTransactions;

public record GetWalletTransactionsQuery(
    string DriverId,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<List<TransactionDto>>>;
