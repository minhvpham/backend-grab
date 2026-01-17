using Driver.Services.Application.Common.Models;

namespace Driver.Services.Application.Common.ExternalServices;

public interface IOrderServiceClient
{
    Task<Result> UpdateOrderStatusAsync(string orderId, string status);
}