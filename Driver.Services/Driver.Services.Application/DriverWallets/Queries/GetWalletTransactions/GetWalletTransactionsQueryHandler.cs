using Driver.Services.Application.Common.Models;
using Driver.Services.Application.DriverWallets.DTOs;
using Driver.Services.Application.DriverWallets.Mappings;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using MediatR;

namespace Driver.Services.Application.DriverWallets.Queries.GetWalletTransactions;

public class GetWalletTransactionsQueryHandler : IRequestHandler<GetWalletTransactionsQuery, Result<List<TransactionDto>>>
{
    private readonly IDriverWalletRepository _walletRepository;

    public GetWalletTransactionsQueryHandler(IDriverWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<Result<List<TransactionDto>>> Handle(
        GetWalletTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByDriverIdAsync(request.DriverId, cancellationToken);

        if (wallet == null)
        {
            return Result.Failure<List<TransactionDto>>(
                Error.NotFound("Wallet.NotFound", $"Wallet for driver '{request.DriverId}' not found."));
        }

        var transactions = wallet.GetTransactions()
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => t.ToDto())
            .ToList();

        return Result.Success(transactions);
    }
}
