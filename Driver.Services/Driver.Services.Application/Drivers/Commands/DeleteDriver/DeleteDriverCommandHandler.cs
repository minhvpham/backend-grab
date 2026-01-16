using Driver.Services.Application.Common.Models;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.DeleteDriver;

public class DeleteDriverCommandHandler : IRequestHandler<DeleteDriverCommand, Result>
{
    private readonly IDriverRepository _driverRepository;

    public DeleteDriverCommandHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<Result> Handle(DeleteDriverCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(request.DriverId, cancellationToken);

        if (driver is null || driver.Deleted)
        {
            return Result.Failure(Error.NotFound("Driver.NotFound", $"Driver with ID '{request.DriverId}' not found"));
        }

        // TODO: Check if driver has active trips or non-zero wallet balance
        // For now, allow deletion

        driver.Delete();
        driver.AddDomainEvent(new DriverDeletedDomainEvent(driver.Id, driver.FullName));

        _driverRepository.Update(driver);

        return Result.Success();
    }
}