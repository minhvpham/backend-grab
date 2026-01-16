using Driver.Services.Application.DriverLocations.Commands.UpdateDriverLocation;
using Driver.Services.Application.DriverLocations.Queries.GetDriverLocation;
using Driver.Services.Application.DriverLocations.Queries.GetNearbyDrivers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Driver.Services.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriverLocationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DriverLocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Update driver's current location
    /// </summary>
    [HttpPut("{driverId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocation(string driverId, [FromBody] UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateDriverLocationCommand(driverId, request.Latitude, request.Longitude);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code == "Driver.NotFound"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Get driver's current location
    /// </summary>
    [HttpGet("{driverId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLocation(string driverId, CancellationToken cancellationToken)
    {
        var query = new GetDriverLocationQuery(driverId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Find nearby drivers within a radius
    /// </summary>
    [HttpGet("nearby")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNearbyDrivers(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 5.0,
        [FromQuery] int maxResults = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetNearbyDriversQuery(latitude, longitude, radiusKm, maxResults);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return Ok(result.Value);
    }
}

public record UpdateLocationRequest(double Latitude, double Longitude);
