using Driver.Services.Application.Common.Models;
using MediatR;

namespace Driver.Services.Application.Drivers.Commands.DeleteDriver;

public record DeleteDriverCommand(Guid DriverId) : IRequest<Result>;