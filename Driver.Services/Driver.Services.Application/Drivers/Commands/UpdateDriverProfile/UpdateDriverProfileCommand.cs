using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.UpdateDriverProfile;

public record UpdateDriverProfileCommand(
    string DriverId,
    string FullName,
    string Email
) : IRequest<Result>;
