using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.EventHandlers;

public class TripDeliveredDomainEventHandler : INotificationHandler<TripDeliveredDomainEvent>
{
    private readonly IDriverWalletRepository _walletRepository;
    private readonly ITripHistoryRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TripDeliveredDomainEventHandler(
        IDriverWalletRepository walletRepository,
        ITripHistoryRepository tripRepository,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(TripDeliveredDomainEvent notification, CancellationToken cancellationToken)
    {
        // Get the completed trip to access fare amount
        var trip = await _tripRepository.GetByIdAsync(notification.TripId, cancellationToken);
        if (trip == null) return;

        // Get or create driver wallet
        var wallet = await _walletRepository.GetByDriverIdAsync(trip.DriverId, cancellationToken);
        if (wallet == null)
        {
            // Create wallet if it doesn't exist
            wallet = DriverWallet.Create(trip.DriverId);
            wallet = _walletRepository.Add(wallet);
        }

        // Add order earning to wallet
        wallet.AddOrderEarning(trip.Fare, trip.OrderId, $"Trip completed: {notification.TripId}");

        // Handle COD collection if cash was collected
        if (notification.CashCollected.HasValue && notification.CashCollected.Value > 0)
        {
            wallet.RecordCashCollection(notification.CashCollected.Value, trip.OrderId);
        }

        await _unitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}