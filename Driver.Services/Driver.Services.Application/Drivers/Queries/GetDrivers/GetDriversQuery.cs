using Driver.Services.Application.Common.Models;
using Driver.Services.Application.Drivers.DTOs;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;

namespace Driver.Services.Application.Drivers.Queries.GetDrivers;

public record GetDriversQuery(
    int PageNumber = 1,
    int PageSize = 10,
    DriverStatus? Status = null,
    VerificationStatus? VerificationStatus = null,
    string? SearchTerm = null
) : IRequest<Result<PaginatedList<DriverSummaryDto>>>;
