using Driver.Services.Application.Common.Models;
using Driver.Services.Application.Drivers.DTOs;
using Driver.Services.Application.Drivers.Mappings;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;

namespace Driver.Services.Application.Drivers.Queries.GetDrivers;

public class GetDriversQueryHandler : IRequestHandler<GetDriversQuery, Result<PaginatedList<DriverSummaryDto>>>
{
    private readonly IDriverRepository _driverRepository;

    public GetDriversQueryHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<Result<PaginatedList<DriverSummaryDto>>> Handle(GetDriversQuery request, CancellationToken cancellationToken)
    {
        // Get all drivers as queryable
        var driversQuery = await _driverRepository.GetAllAsync(cancellationToken);

        // Apply filters
        if (request.Status.HasValue)
        {
            driversQuery = driversQuery.Where(d => d.Status == request.Status.Value);
        }

        if (request.VerificationStatus.HasValue)
        {
            driversQuery = driversQuery.Where(d => d.VerificationStatus == request.VerificationStatus.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            driversQuery = driversQuery.Where(d =>
                d.FullName.ToLower().Contains(searchTerm) ||
                d.Email.ToLower().Contains(searchTerm) ||
                d.PhoneNumber.Value.Contains(searchTerm));
        }

        // Convert to DTO and create paginated result
        var driverDtos = driversQuery.Select(d => d.ToSummaryDto());
        var paginatedResult = await PaginatedList<DriverSummaryDto>.CreateAsync(
            driverDtos,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result.Success(paginatedResult);
    }
}
