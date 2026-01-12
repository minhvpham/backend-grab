using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.UpdateDriverProfile;

public record UpdateDriverProfileCommand(
    Guid DriverId,
    string FullName,
    string Email,
    string? ProfileImage = null
) : IRequest<Result>;
