using Driver.Services.Application.Common.Models;
using Driver.Services.Application.Drivers.DTOs;
using MediatR;

namespace Driver.Services.Application.Drivers.Queries.GetDriverById;

public record GetDriverByIdQuery(string DriverId) : IRequest<Result<DriverDto>>;
