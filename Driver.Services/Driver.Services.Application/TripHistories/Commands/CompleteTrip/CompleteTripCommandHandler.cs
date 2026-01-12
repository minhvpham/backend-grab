using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.Commands.CompleteTrip;

public class CompleteTripCommandHandler : IRequestHandler<CompleteTripCommand, Result>
{
    private readonly ITripHistoryRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteTripCommandHandler(
        ITripHistoryRepository tripRepository,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CompleteTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        
        if (trip == null)
        {
            return Result.Failure(
                Error.NotFound("Trip.NotFound", $"Trip with ID '{request.TripId}' not found."));
        }

        try
        {
            trip.CompleteDelivery(request.CashCollected, request.DriverNotes);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Validation("Trip.CompleteFailed", ex.Message));
        }
    }
}
