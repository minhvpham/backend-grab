using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.VerifyDriver;

public record VerifyDriverCommand(Guid DriverId) : IRequest<Result>;
