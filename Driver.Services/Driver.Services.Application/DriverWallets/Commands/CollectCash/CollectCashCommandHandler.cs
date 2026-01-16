using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using MediatR;

namespace Driver.Services.Application.DriverWallets.Commands.CollectCash;

public class CollectCashCommandHandler : IRequestHandler<CollectCashCommand, Result>
{
    private readonly IDriverWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CollectCashCommandHandler(
        IDriverWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CollectCashCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByDriverIdAsync(request.DriverId, cancellationToken);
        
        if (wallet == null)
        {
            return Result.Failure(
                Error.NotFound("Wallet.NotFound", $"Wallet for driver '{request.DriverId}' not found."));
        }

        try
        {
            wallet.RecordCashCollection(request.Amount, request.OrderId, request.Reference);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Validation("Wallet.CollectCashFailed", ex.Message));
        }
    }
}
