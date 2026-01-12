using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using MediatR;

namespace Driver.Services.Application.TripHistories.Commands.UpdateTripStatus;

public class UpdateTripStatusCommandHandler : IRequestHandler<UpdateTripStatusCommand, Result>
{
    private readonly ITripHistoryRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTripStatusCommandHandler(
        ITripHistoryRepository tripRepository,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTripStatusCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);
        
        if (trip == null)
        {
            return Result.Failure(
                Error.NotFound("Trip.NotFound", $"Trip with ID '{request.TripId}' not found."));
        }

        try
        {
            switch (request.Action.ToLower())
            {
                case "accept":
                    trip.Accept();
                    break;
                case "pickup":
                    trip.MarkAsPickedUp();
                    break;
                case "start_delivery":
                    trip.StartDelivery();
                    break;
                default:
                    return Result.Failure(
                        Error.Validation("Trip.InvalidAction", $"Invalid action: {request.Action}"));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(
                Error.Validation("Trip.UpdateStatusFailed", ex.Message));
        }
    }
}
