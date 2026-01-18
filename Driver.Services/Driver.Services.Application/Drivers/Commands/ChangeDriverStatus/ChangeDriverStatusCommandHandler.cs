using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.ChangeDriverStatus;

public class ChangeDriverStatusCommandHandler : IRequestHandler<ChangeDriverStatusCommand, Result>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeDriverStatusCommandHandler(
        IDriverRepository driverRepository,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeDriverStatusCommand request, CancellationToken cancellationToken)
    {
        // Check if driver exists
        var driver = await _driverRepository.GetByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure(
                Error.NotFound("Driver.NotFound", $"Driver with ID '{request.DriverId}' not found."));
        }

        // Change driver status based on requested status
        try
        {
            switch (request.NewStatus)
            {
                case DriverStatus.Online:
                    driver.GoOnline();
                    break;
                case DriverStatus.Offline:
                    driver.GoOffline();
                    break;
                case DriverStatus.Busy:
                    driver.MarkAsBusy();
                    break;
                case DriverStatus.WaitingForAcceptance:
                    driver.MarkAsWaitingForAcceptance();
                    break;
                default:
                    return Result.Failure(
                        Error.Validation("Driver.InvalidStatus", $"Invalid driver status: {request.NewStatus}"));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(
                Error.Validation("Driver.ChangeStatus", ex.Message));
        }
    }
}
