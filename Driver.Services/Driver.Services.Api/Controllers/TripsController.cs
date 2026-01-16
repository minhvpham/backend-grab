using Driver.Services.Application.TripHistories.Commands.CreateTrip;
using Driver.Services.Application.TripHistories.Commands.UpdateTripStatus;
using Driver.Services.Application.TripHistories.Commands.CompleteTrip;
using Driver.Services.Application.TripHistories.Commands.CancelTrip;
using Driver.Services.Application.TripHistories.Queries.GetTripById;
using Driver.Services.Application.TripHistories.Queries.GetDriverTrips;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Driver.Services.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TripsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new trip (assign order to driver)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTrip([FromBody] CreateTripCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return CreatedAtAction(nameof(GetTripById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Get trip by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTripById(string id, CancellationToken cancellationToken)
    {
        var query = new GetTripByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Get driver's trips
    /// </summary>
    [HttpGet("driver/{driverId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDriverTrips(
        string driverId,
        [FromQuery] bool activeOnly = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDriverTripsQuery(driverId, activeOnly, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Update trip status (accept, pickup, start_delivery)
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTripStatusCommand(id, request.Action);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code == "Trip.NotFound"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Complete trip (mark as delivered)
    /// </summary>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTrip(string id, [FromBody] CompleteTripRequest request, CancellationToken cancellationToken)
    {
        var command = new CompleteTripCommand(id, request.CashCollected, request.DriverNotes);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code == "Trip.NotFound"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Cancel trip
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelTrip(string id, [FromBody] CancelTripRequest request, CancellationToken cancellationToken)
    {
        var command = new CancelTripCommand(id, request.Reason);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code == "Trip.NotFound"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return NoContent();
    }
}

public record UpdateStatusRequest(string Action);
public record CompleteTripRequest(decimal? CashCollected, string? DriverNotes);
public record CancelTripRequest(string Reason);
