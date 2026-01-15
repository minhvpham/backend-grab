using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using MediatR;

namespace Driver.Services.Application.DriverWallets.Commands.AddFunds;

public class AddFundsCommandHandler : IRequestHandler<AddFundsCommand, Result>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddFundsCommandHandler(
        IDriverRepository driverRepository,
        IDriverWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddFundsCommand request, CancellationToken cancellationToken)
    {
        // Verify driver exists
        var driver = await _driverRepository.GetByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure(
                Error.NotFound("Driver.NotFound", $"Driver with ID '{request.DriverId}' not found."));
        }

        try
        {
            // Get or create wallet
            var wallet = await _walletRepository.GetByDriverIdAsync(request.DriverId, cancellationToken);
            
            if (wallet == null)
            {
                wallet = DriverWallet.Create(request.DriverId);
                _walletRepository.Add(wallet);
            }

            // Add funds
            wallet.Deposit(request.Amount, request.Reference, request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Validation("Wallet.AddFundsFailed", ex.Message));
        }
    }
}
