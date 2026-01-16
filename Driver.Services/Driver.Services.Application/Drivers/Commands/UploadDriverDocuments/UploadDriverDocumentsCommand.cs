using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.UploadDriverDocuments;

public record UploadDriverDocumentsCommand(
    string DriverId,
    string? CitizenIdImageUrl = null,
    string? DriverLicenseImageUrl = null,
    string? DriverRegistrationImageUrl = null) : IRequest<Result<Unit>>;