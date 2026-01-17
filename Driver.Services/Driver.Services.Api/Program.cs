using Driver.Services.Application;
using Driver.Services.Application.Common.ExternalServices;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using Driver.Services.Infrastructure.Persistence;
using Driver.Services.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Driver Services API", Version = "v1" });
});

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string 'DefaultConnection' not found."
    );
}

builder.Services.AddDbContext<DriverServicesDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Add Repositories
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IDriverLocationRepository, DriverLocationRepository>();
builder.Services.AddScoped<IDriverWalletRepository, DriverWalletRepository>();
builder.Services.AddScoped<ITripHistoryRepository, TripHistoryRepository>();

// Add UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add MediatR and Application layer dependencies
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(Driver.Services.Application.AssemblyReference).Assembly
    );
    // Register pipeline behaviors
    cfg.AddOpenBehavior(typeof(Driver.Services.Application.Common.Behaviours.LoggingBehaviour<,>));
    cfg.AddOpenBehavior(
        typeof(Driver.Services.Application.Common.Behaviours.ValidationBehaviour<,>)
    );
    cfg.AddOpenBehavior(
        typeof(Driver.Services.Application.Common.Behaviours.TransactionBehaviour<,>)
    );
});
builder.Services.AddValidatorsFromAssembly(
    typeof(Driver.Services.Application.AssemblyReference).Assembly
);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Add HTTP client with retry policy for Order.Service
builder.Services.AddHttpClient("OrderService", client =>
{
    var orderServiceUrl = builder.Configuration["ORDER_SERVICE_URL"] ?? "http://order-service:8002";
    client.BaseAddress = new Uri(orderServiceUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("Content-Type", "application/json");
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

// Add Order.Service client
builder.Services.AddScoped<IOrderServiceClient, OrderServiceClient>();

var app = builder.Build();

// Define retry and circuit breaker policies
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DriverServicesDbContext>();

    // Apply pending migrations
    await context.Database.MigrateAsync();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Driver Services API v1"));
}

app.UseHttpsRedirection();
app.UseCors();

// Ensure uploads directory exists
Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "uploads"));

// Serve static files from uploads directory
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "uploads")),
    RequestPath = "/uploads"
});

app.UseAuthorization();

// Map controllers
app.MapControllers();

// Health check endpoint
app.MapGet(
        "/health",
        () => Results.Ok(new { status = "healthy", timestamp = DateTimeOffset.UtcNow })
    )
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();
