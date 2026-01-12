using Driver.Services.Application.Common.Models;
using Driver.Services.Application.DriverWallets.DTOs;
using Driver.Services.Application.DriverWallets.Mappings;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using MediatR;

namespace Driver.Services.Application.DriverWallets.Queries.GetWalletBalance;

public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, Result<WalletBalanceDto>>
{
    private readonly IDriverWalletRepository _walletRepository;

    public GetWalletBalanceQueryHandler(IDriverWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<Result<WalletBalanceDto>> Handle(
        GetWalletBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByDriverIdAsync(request.DriverId, cancellationToken);

        if (wallet == null)
        {
            return Result.Failure<WalletBalanceDto>(
                Error.NotFound("Wallet.NotFound", $"Wallet for driver '{request.DriverId}' not found."));
        }

        return Result.Success(wallet.ToBalanceDto());
    }
}
