using Driver.Services.Api.Middleware;
using Driver.Services.Application;
using Driver.Services.Application.Common.Behaviours;
using Driver.Services.Domain.Abstractions;
using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using Driver.Services.Infrastructure.Persistence;
using Driver.Services.Infrastructure.Persistence.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Driver Services API", Version = "v1" });
});

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string 'DefaultConnection' not found.");
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
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Driver.Services.Application.AssemblyReference).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Driver.Services.Application.AssemblyReference).Assembly);

// Add MediatR Pipeline Behaviors (order matters!)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Add Global Exception Handler Middleware (must be first!)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Driver Services API v1"));
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();

// Map controllers
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTimeOffset.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();

