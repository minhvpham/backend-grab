using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.RejectDriver;

public class RejectDriverCommandHandler : IRequestHandler<RejectDriverCommand, Result>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectDriverCommandHandler(
        IDriverRepository driverRepository,
        IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RejectDriverCommand request, CancellationToken cancellationToken)
    {
        // Check if driver exists
        var driver = await _driverRepository.GetByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure(
                Error.NotFound("Driver.NotFound", $"Driver with ID '{request.DriverId}' not found."));
        }

        // Reject the driver with reason
        try
        {
            driver.Reject(request.RejectionReason);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(
                Error.Validation("Driver.Reject", ex.Message));
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(
                Error.Validation("Driver.Reject", ex.Message));
        }
    }
}
