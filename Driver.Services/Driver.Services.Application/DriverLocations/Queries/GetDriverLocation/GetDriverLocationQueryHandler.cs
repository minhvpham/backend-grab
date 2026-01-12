using Driver.Services.Application.Common.Models;
using Driver.Services.Application.DriverLocations.DTOs;
using Driver.Services.Application.DriverLocations.Mappings;
using Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;
using MediatR;

namespace Driver.Services.Application.DriverLocations.Queries.GetDriverLocation;

public class GetDriverLocationQueryHandler : IRequestHandler<GetDriverLocationQuery, Result<DriverLocationDto>>
{
    private readonly IDriverLocationRepository _driverLocationRepository;

    public GetDriverLocationQueryHandler(IDriverLocationRepository driverLocationRepository)
    {
        _driverLocationRepository = driverLocationRepository;
    }

    public async Task<Result<DriverLocationDto>> Handle(
        GetDriverLocationQuery request,
        CancellationToken cancellationToken)
    {
        var driverLocation = await _driverLocationRepository.GetLatestByDriverIdAsync(request.DriverId, cancellationToken);

        if (driverLocation == null)
        {
            return Result.Failure<DriverLocationDto>(
                Error.NotFound("DriverLocation.NotFound", $"Location for driver '{request.DriverId}' not found."));
        }

        return Result.Success(driverLocation.ToDto());
    }
}
