using Driver.Services.Application.Drivers.Commands.RegisterDriver;
using Driver.Services.Application.Drivers.Commands.UpdateDriverProfile;
using Driver.Services.Application.Drivers.Commands.UpdateVehicleInfo;
using Driver.Services.Application.Drivers.Commands.VerifyDriver;
using Driver.Services.Application.Drivers.Commands.RejectDriver;
using Driver.Services.Application.Drivers.Commands.ChangeDriverStatus;
using Driver.Services.Application.Drivers.Queries.GetDriverById;
using Driver.Services.Application.Drivers.Queries.GetDrivers;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Driver.Services.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly IMediator _mediator;

    public DriversController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new driver
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDriver([FromBody] RegisterDriverCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return CreatedAtAction(nameof(GetDriverById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Get driver by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDriverById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetDriverByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Get list of drivers with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDrivers(
        [FromQuery] DriverStatus? status = null,
        [FromQuery] VerificationStatus? verificationStatus = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDriversQuery(pageNumber, pageSize, status, verificationStatus, searchTerm);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Update driver profile
    /// </summary>
    [HttpPut("{id}/profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateDriverProfileRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateDriverProfileCommand(
            id,
            request.FullName,
            request.Email,
            request.ProfileImageUrl);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code == "Driver.NotFound" 
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Update vehicle information
    /// </summary>
    [HttpPut("{id}/vehicle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] UpdateVehicleInfoRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateVehicleInfoCommand(
            id,
            request.VehicleType,
            request.LicensePlate,
            request.VehicleMake,
            request.VehicleModel,
            request.VehicleColor,
            request.VehicleYear);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code == "Driver.NotFound"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Verify driver (admin only)
    /// </summary>
    [HttpPost("{id}/verify")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyDriver(Guid id, CancellationToken cancellationToken)
    {
        var command = new VerifyDriverCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Reject driver (admin only)
    /// </summary>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectDriver(Guid id, [FromBody] RejectDriverRequest request, CancellationToken cancellationToken)
    {
        var command = new RejectDriverCommand(id, request.Reason);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code == "Driver.NotFound"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Change driver status
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new ChangeDriverStatusCommand(id, request.Status);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code == "Driver.NotFound"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return NoContent();
    }
}

// Request models
public record UpdateDriverProfileRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string? ProfileImageUrl);

public record UpdateVehicleInfoRequest(
    string VehicleType,
    string VehicleMake,
    string VehicleModel,
    int VehicleYear,
    string LicensePlate,
    string? VehicleColor);

public record RejectDriverRequest(string Reason);

public record ChangeStatusRequest(DriverStatus Status);
