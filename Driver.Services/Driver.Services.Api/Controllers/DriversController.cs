using Driver.Services.Application.Drivers.Commands.RegisterDriver;
using Driver.Services.Application.Drivers.Commands.UpdateDriverProfile;
using Driver.Services.Application.Drivers.Commands.UpdateVehicleInfo;
using Driver.Services.Application.Drivers.Commands.VerifyDriver;
using Driver.Services.Application.Drivers.Commands.RejectDriver;
using Driver.Services.Application.Drivers.Commands.ChangeDriverStatus;
using Driver.Services.Application.Drivers.Commands.DeleteDriver;
using Driver.Services.Application.Drivers.Commands.UploadDriverDocuments;
using Driver.Services.Application.Drivers.Queries.GetDriverById;
using Driver.Services.Application.Drivers.Queries.GetDrivers;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

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

    private async Task<string?> SaveFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Invalid file type. Only image files are allowed.");

        // Validate file size (5MB max)
        const long maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
            throw new ArgumentException("File size exceeds the maximum allowed size of 5MB.");

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine("uploads", fileName);

        // Ensure directory exists
        Directory.CreateDirectory("uploads");

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{fileName}";
    }

    /// <summary>
    /// Register a new driver
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDriver([FromForm] RegisterDriverRequest request, CancellationToken cancellationToken)
    {
        // Process uploaded files
        var citizenIdPath = request.CitizenIdImage != null ? await SaveFileAsync(request.CitizenIdImage) : null;
        var licensePath = request.DriverLicenseImage != null ? await SaveFileAsync(request.DriverLicenseImage) : null;
        var registrationPath = request.DriverRegistrationImage != null ? await SaveFileAsync(request.DriverRegistrationImage) : null;

        var command = new RegisterDriverCommand(
            request.FullName,
            request.PhoneNumber,
            request.Email,
            request.LicenseNumber,
            citizenIdPath,
            licensePath,
            registrationPath,
            request.DriverId);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return CreatedAtAction(nameof(GetDriverById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Get driver by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDriverById(string id, CancellationToken cancellationToken)
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
    /// Update driver profile (scalar data only, no file handling)
    /// </summary>
    [HttpPut("{id}/profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateDriverProfileRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateDriverProfileCommand(
            id,
            request.FullName,
            request.Email);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code == "Driver.NotFound"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Upload driver verification documents
    /// </summary>
    [HttpPost("{id}/upload-documents")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadDocuments(string id, [FromForm] UploadDocumentsRequest request, CancellationToken cancellationToken)
    {
        // Process uploaded files
        var citizenIdPath = request.CitizenIdImage != null ? await SaveFileAsync(request.CitizenIdImage) : null;
        var licensePath = request.DriverLicenseImage != null ? await SaveFileAsync(request.DriverLicenseImage) : null;
        var registrationPath = request.DriverRegistrationImage != null ? await SaveFileAsync(request.DriverRegistrationImage) : null;

        var command = new UploadDriverDocumentsCommand(
            id,
            citizenIdPath,
            licensePath,
            registrationPath);

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
    public async Task<IActionResult> UpdateVehicle(string id, [FromBody] UpdateVehicleInfoRequest request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> VerifyDriver(string id, CancellationToken cancellationToken)
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
    public async Task<IActionResult> RejectDriver(string id, [FromBody] RejectDriverRequest request, CancellationToken cancellationToken)
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
    public async Task<IActionResult> ChangeStatus(string id, [FromBody] ChangeStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new ChangeDriverStatusCommand(id, request.Status);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code == "Driver.NotFound"
                ? NotFound(new { error = result.Error.Message })
                : BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Delete driver (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDriver(string id, CancellationToken cancellationToken)
    {
        var command = new DeleteDriverCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });

        return NoContent();
    }
}

// Request models
public record RegisterDriverRequest(
    string FullName,
    string PhoneNumber,
    string Email,
    string LicenseNumber,
    IFormFile? CitizenIdImage,
    IFormFile? DriverLicenseImage,
    IFormFile? DriverRegistrationImage,
    string? DriverId = null);

public record UploadDocumentsRequest(
    IFormFile? CitizenIdImage,
    IFormFile? DriverLicenseImage,
    IFormFile? DriverRegistrationImage);

public record UpdateDriverProfileRequest(
    string FullName,
    string Email,
    string PhoneNumber);

public record UpdateVehicleInfoRequest(
    string VehicleType,
    string VehicleMake,
    string VehicleModel,
    int VehicleYear,
    string LicensePlate,
    string? VehicleColor);

public record RejectDriverRequest(string Reason);

public record ChangeStatusRequest(DriverStatus Status);
