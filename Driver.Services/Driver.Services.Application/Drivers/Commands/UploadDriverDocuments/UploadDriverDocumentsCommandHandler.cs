using Driver.Services.Application.Common.Models;
using Driver.Services.Application.Drivers.Commands.UploadDriverDocuments;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.UploadDriverDocuments;

public class UploadDriverDocumentsCommandHandler : IRequestHandler<UploadDriverDocumentsCommand, Result<Unit>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadDriverDocumentsCommandHandler(IDriverRepository driverRepository, IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(UploadDriverDocumentsCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(request.DriverId);
        if (driver == null)
            return Result.Failure<Unit>(Error.Failure("Driver.NotFound", "Driver not found"));

        driver.UpdateProfile(
            citizenIdImageUrl: request.CitizenIdImageUrl,
            driverLicenseImageUrl: request.DriverLicenseImageUrl,
            driverRegistrationImageUrl: request.DriverRegistrationImageUrl);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}