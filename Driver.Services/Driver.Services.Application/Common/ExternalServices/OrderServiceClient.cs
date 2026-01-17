using Driver.Services.Application.Common.ExternalServices;
using Driver.Services.Application.Common.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Driver.Services.Application.Common.ExternalServices;

public class OrderServiceClient : IOrderServiceClient
{
    private readonly HttpClient _httpClient;

    public OrderServiceClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("OrderService");
    }

    public async Task<Result> UpdateOrderStatusAsync(string orderId, string status)
    {
        try
        {
            var requestBody = new { status };
            var response = await _httpClient.PutAsJsonAsync($"/orders/{orderId}", requestBody);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return Result.Failure(Error.Failure("OrderService.UpdateFailed",
                $"Failed to update order status: {response.StatusCode} - {errorContent}"));
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure(Error.Failure("OrderService.ConnectionFailed",
                $"Unable to connect to Order.Service: {ex.Message}"));
        }
        catch (TaskCanceledException)
        {
            return Result.Failure(Error.Failure("OrderService.Timeout",
                "Request to Order.Service timed out"));
        }
        catch (JsonException ex)
        {
            return Result.Failure(Error.Failure("OrderService.InvalidResponse",
                $"Invalid response from Order.Service: {ex.Message}"));
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Failure("OrderService.UnexpectedError",
                $"Unexpected error calling Order.Service: {ex.Message}"));
        }
    }
}