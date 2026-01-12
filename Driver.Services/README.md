# Driver Services API

A complete microservice for managing drivers, their locations, wallets, and trip histories built with Clean Architecture, CQRS, and Domain-Driven Design.

## Features

- **Driver Management**: Register, verify, and manage driver profiles
- **Location Tracking**: Real-time GPS location tracking with nearby driver search
- **Wallet System**: Digital wallet with transactions, COD handling, and balance management
- **Trip History**: Complete order delivery lifecycle tracking

## Architecture

- **Clean Architecture** with separated layers (Domain, Application, Infrastructure, API)
- **CQRS Pattern** with MediatR for command/query separation
- **Domain-Driven Design** with aggregates, value objects, and domain events
- **Repository Pattern** with Unit of Work for data access
- **Entity Framework Core** with PostgreSQL database

## Tech Stack

- **.NET 8.0**
- **ASP.NET Core Web API**
- **Entity Framework Core 8.0**
- **PostgreSQL 16**
- **MediatR** for CQRS
- **FluentValidation** for validation
- **Swagger/OpenAPI** for API documentation

## Quick Start

### Prerequisites

- Docker & Docker Compose

### Run with Docker Compose

```bash
# Start the entire application stack (PostgreSQL + API)
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down

# Stop and remove volumes (clean database)
docker-compose down -v
```

The API will be available at:
- **API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger
- **Health Check**: http://localhost:8080/health

The database migrations are automatically applied on startup!

## Development Setup (Without Docker)

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL 16
- dotnet-ef tool: `dotnet tool install --global dotnet-ef`

### Steps

1. **Update Connection String** (if needed):
   ```bash
   # Edit Driver.Services.Api/appsettings.json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=driver_services;Username=postgres;Password=postgres"
   }
   ```

2. **Apply Migrations**:
   ```bash
   cd Driver.Services
   dotnet ef database update --project Driver.Services.Infrastructure --startup-project Driver.Services.Api
   ```

3. **Run the API**:
   ```bash
   cd Driver.Services.Api
   dotnet run
   ```

4. **Access Swagger**: https://localhost:5001/swagger

## API Endpoints

### Drivers
- `POST /api/drivers` - Register new driver
- `GET /api/drivers/{id}` - Get driver by ID
- `GET /api/drivers` - Get drivers with filters (status, verification, pagination)
- `PUT /api/drivers/{id}/profile` - Update driver profile
- `PUT /api/drivers/{id}/vehicle` - Update vehicle information
- `POST /api/drivers/{id}/verify` - Verify driver
- `POST /api/drivers/{id}/reject` - Reject driver verification
- `PATCH /api/drivers/{id}/status` - Change driver status

### Driver Locations
- `POST /api/drivers/{driverId}/location` - Update driver location
- `GET /api/drivers/{driverId}/location` - Get driver current location
- `GET /api/drivers/nearby` - Find nearby drivers (radius search)

### Driver Wallets
- `GET /api/drivers/{driverId}/wallet/balance` - Get wallet balance
- `GET /api/drivers/{driverId}/wallet/transactions` - Get transaction history
- `POST /api/drivers/{driverId}/wallet/add-funds` - Add funds to wallet
- `POST /api/drivers/{driverId}/wallet/collect-cash` - Record COD collection
- `POST /api/drivers/{driverId}/wallet/return-cash` - Return cash to balance

### Trips
- `POST /api/trips` - Create new trip
- `GET /api/trips/{id}` - Get trip by ID
- `GET /api/trips/driver/{driverId}` - Get driver trips
- `PATCH /api/trips/{id}/status` - Update trip status
- `POST /api/trips/{id}/complete` - Complete trip
- `POST /api/trips/{id}/cancel` - Cancel trip

## Project Structure

```
Driver.Services/
├── Driver.Services.Domain/          # Domain entities, value objects, interfaces
│   ├── AggregatesModel/
│   │   ├── DriverAggregate/
│   │   ├── DriverLocationAggregate/
│   │   ├── DriverWalletAggregate/
│   │   └── TripHistoryAggregate/
│   ├── Abstractions/
│   └── Exceptions/
├── Driver.Services.Application/     # CQRS commands, queries, handlers
│   ├── Common/
│   ├── Drivers/
│   ├── DriverLocations/
│   ├── DriverWallets/
│   └── TripHistories/
├── Driver.Services.Infrastructure/  # EF Core, repositories, persistence
│   └── Persistence/
│       ├── Configurations/
│       ├── Repositories/
│       └── Migrations/
├── Driver.Services.Api/             # REST API controllers, middleware
│   └── Controllers/
├── docker-compose.yml
└── Dockerfile
```

## Database Schema

### Tables
- **Drivers** - Driver profiles with vehicle info
- **DriverLocations** - GPS location tracking
- **DriverWallets** - Wallet balances and totals
- **Transactions** - Financial transaction history
- **TripHistories** - Order delivery lifecycle

## Configuration

### Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `ASPNETCORE_URLS` - Listening URLs (default: http://+:8080)
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=driver_services;Username=postgres;Password=postgres"
  }
}
```

## Docker Commands

```bash
# Build image
docker-compose build

# View logs
docker-compose logs -f

# Restart API only
docker-compose restart api

# Access database
docker exec -it driver-services-db psql -U postgres -d driver_services

# View running containers
docker-compose ps
```

## Health Checks

The API includes a health endpoint:

```bash
curl http://localhost:8080/health
```

Response:
```json
{
  "status": "healthy",
  "timestamp": "2026-01-12T17:44:18.123Z"
}
```

## License

MIT License
