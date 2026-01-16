using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using MediatR;

namespace Driver.Services.Application.DriverWallets.Commands.ReturnCash;

public class ReturnCashCommandHandler : IRequestHandler<ReturnCashCommand, Result>
{
    private readonly IDriverWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReturnCashCommandHandler(
        IDriverWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReturnCashCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByDriverIdAsync(request.DriverId, cancellationToken);
        
        if (wallet == null)
        {
            return Result.Failure(
                Error.NotFound("Wallet.NotFound", $"Wallet for driver '{request.DriverId}' not found."));
        }

        try
        {
            wallet.ReturnCash(request.Amount, request.Reference, request.Description);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Validation("Wallet.ReturnCashFailed", ex.Message));
        }
    }
}
