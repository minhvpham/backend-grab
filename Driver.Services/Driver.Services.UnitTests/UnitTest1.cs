using Driver.Services.Application.TripHistories.EventHandlers;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using Driver.Services.Domain.Abstractions;
using Moq;
using Xunit;

namespace Driver.Services.UnitTests;

public class TripDeliveredDomainEventHandlerTests
{
    private readonly Mock<IDriverWalletRepository> _walletRepositoryMock;
    private readonly Mock<ITripHistoryRepository> _tripRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TripDeliveredDomainEventHandler _handler;

    public TripDeliveredDomainEventHandlerTests()
    {
        _walletRepositoryMock = new Mock<IDriverWalletRepository>();
        _tripRepositoryMock = new Mock<ITripHistoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new TripDeliveredDomainEventHandler(
            _walletRepositoryMock.Object,
            _tripRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_Should_AddOrderEarning_When_WalletExists()
    {
        // Arrange
        var driverId = "driver-123";
        var tripId = "trip-123";
        var orderId = "order-123";
        var fare = 50.00m;
        var cashCollected = 50.00m;

        var trip = TripHistory.Create(
            driverId: driverId,
            orderId: orderId,
            pickupAddress: "Pickup Address",
            pickupLat: 10.0,
            pickupLng: 20.0,
            deliveryAddress: "Delivery Address",
            deliveryLat: 15.0,
            deliveryLng: 25.0,
            fare: fare);

        var existingWallet = DriverWallet.Create(driverId);
        _walletRepositoryMock.Setup(x => x.Add(existingWallet)).Returns(existingWallet);

        _tripRepositoryMock.Setup(x => x.GetByIdAsync(tripId, default))
            .ReturnsAsync(trip);
        _walletRepositoryMock.Setup(x => x.GetByDriverIdAsync(driverId, default))
            .ReturnsAsync(existingWallet);

        var domainEvent = new TripDeliveredDomainEvent(tripId, driverId, orderId, cashCollected);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _walletRepositoryMock.Verify(x => x.GetByDriverIdAsync(driverId, default), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveEntitiesAsync(default), Times.Once);

        // Verify transactions were added (OrderEarning + CashCollection)
        Assert.Equal(2, existingWallet.Transactions.Count);

        // Verify wallet balance was updated
        // Balance should be 0 because earnings (50) are offset by COD collection (50)
        Assert.Equal(0, existingWallet.Balance);
        Assert.Equal(fare, existingWallet.TotalEarnings);
        Assert.Equal(cashCollected, existingWallet.CashOnHand);

        // Verify transaction was added
        var transaction = existingWallet.Transactions.First();
        Assert.Equal(TransactionType.OrderEarning, transaction.Type);
        Assert.Equal(fare, transaction.Amount);
    }

    [Fact]
    public async Task Handle_Should_CreateWallet_When_WalletDoesNotExist()
    {
        // Arrange
        var driverId = "driver-123";
        var tripId = "trip-123";
        var orderId = "order-123";
        var fare = 30.00m;

        var trip = TripHistory.Create(
            driverId: driverId,
            orderId: orderId,
            pickupAddress: "Pickup Address",
            pickupLat: 10.0,
            pickupLng: 20.0,
            deliveryAddress: "Delivery Address",
            deliveryLat: 15.0,
            deliveryLng: 25.0,
            fare: fare);

        var newWallet = DriverWallet.Create(driverId);

        _tripRepositoryMock.Setup(x => x.GetByIdAsync(tripId, default))
            .ReturnsAsync(trip);
        _walletRepositoryMock.Setup(x => x.GetByDriverIdAsync(driverId, default))
            .ReturnsAsync((DriverWallet?)null);
        _walletRepositoryMock.Setup(x => x.Add(It.IsAny<DriverWallet>())).Returns(newWallet);

        var domainEvent = new TripDeliveredDomainEvent(tripId, driverId, orderId, null);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _walletRepositoryMock.Verify(x => x.Add(It.Is<DriverWallet>(w => w.DriverId == driverId)), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveEntitiesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_DoNothing_When_TripNotFound()
    {
        // Arrange
        var driverId = "driver-123";
        var tripId = "trip-123";
        var orderId = "order-123";

        _tripRepositoryMock.Setup(x => x.GetByIdAsync(tripId, default))
            .ReturnsAsync((TripHistory?)null);

        var domainEvent = new TripDeliveredDomainEvent(tripId, driverId, orderId, null);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _walletRepositoryMock.Verify(x => x.GetByDriverIdAsync(It.IsAny<string>(), default), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveEntitiesAsync(default), Times.Never);
    }
}
