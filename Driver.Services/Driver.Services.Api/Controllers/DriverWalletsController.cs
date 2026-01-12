using Driver.Services.Application.DriverWallets.Commands.AddFunds;
using Driver.Services.Application.DriverWallets.Commands.CollectCash;
using Driver.Services.Application.DriverWallets.Commands.ReturnCash;
using Driver.Services.Application.DriverWallets.Queries.GetWalletBalance;
using Driver.Services.Application.DriverWallets.Queries.GetWalletTransactions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Driver.Services.Api.Controllers;

[ApiController]
[Route("api/drivers/{driverId}/wallet")]
public class DriverWalletsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DriverWalletsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get driver wallet balance
    /// </summary>
    [HttpGet("balance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBalance(Guid driverId, CancellationToken cancellationToken)
    {
        var query = new GetWalletBalanceQuery(driverId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Get wallet transaction history
    /// </summary>
    [HttpGet("transactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(
        Guid driverId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetWalletTransactionsQuery(driverId, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(new { error = result.Error.Message });

        return Ok(result.Value);
    }

    /// <summary>
    /// Add funds to wallet (deposit)
    /// </summary>
    [HttpPost("add-funds")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddFunds(Guid driverId, [FromBody] AddFundsRequest request, CancellationToken cancellationToken)
    {
        var command = new AddFundsCommand(driverId, request.Amount, request.Reference, request.Description);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Collect cash (COD) - deducts from balance, adds to cash on hand
    /// </summary>
    [HttpPost("collect-cash")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CollectCash(Guid driverId, [FromBody] CollectCashRequest request, CancellationToken cancellationToken)
    {
        var command = new CollectCashCommand(driverId, request.Amount, request.OrderId, request.Reference);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return NoContent();
    }

    /// <summary>
    /// Return cash to balance (deposit COD collected)
    /// </summary>
    [HttpPost("return-cash")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReturnCash(Guid driverId, [FromBody] ReturnCashRequest request, CancellationToken cancellationToken)
    {
        var command = new ReturnCashCommand(driverId, request.Amount, request.Reference, request.Description);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Message });

        return NoContent();
    }
}

public record AddFundsRequest(decimal Amount, string? Reference, string? Description);
public record CollectCashRequest(decimal Amount, string OrderId, string? Reference);
public record ReturnCashRequest(decimal Amount, string? Reference, string? Description);
