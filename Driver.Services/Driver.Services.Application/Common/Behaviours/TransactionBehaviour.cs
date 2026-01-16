using Driver.Services.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Driver.Services.Application.Common.Behaviours;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;

    public TransactionBehaviour(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Check if this is a query (queries don't need transactions)
        if (requestName.Contains("Query") || requestName.Contains("Get"))
        {
            return await next();
        }

        _logger.LogInformation(
            "Starting transaction for {RequestName}",
            requestName);

        try
        {
            var response = await next();
            
            await _unitOfWork.SaveEntitiesAsync(cancellationToken);
            
            _logger.LogInformation(
                "Committed transaction for {RequestName}",
                requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Transaction failed for {RequestName}",
                requestName);

            throw;
        }
    }
}
