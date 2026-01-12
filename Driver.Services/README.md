# Driver Services API

A microservice for managing driver operations in a delivery/ride-hailing system, built with Clean Architecture principles using .NET 8.

## Architecture

This project follows **Clean Architecture** with the following layers:

- **Driver.Services.Domain**: Core business logic and domain entities
- **Driver.Services.Application**: Use cases and application logic (CQRS with MediatR)
- **Driver.Services.Infrastructure**: Data access and external dependencies (EF Core + PostgreSQL)
- **Driver.Services.Api**: Web API endpoints (REST)

## Core Features

1. **Driver Profile Management**: Registration, verification, vehicle information
2. **Driver Status Management**: Online/Offline/Busy status tracking
3. **Driver Location Tracking**: Real-time GPS location updates
4. **Driver Wallet Management**: COD balance, earnings, withdrawals
5. **Order Dispatching**: Assign orders to drivers
6. **Trip History**: Order tracking and earnings calculation

## Technology Stack

- **.NET 8** - Runtime framework
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - Primary database
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **JWT Authentication** - Security
- **Swagger/OpenAPI** - API documentation

## Getting Started

### Prerequisites

- Docker & Docker Compose (for containerized setup)
- .NET 8 SDK (for local development)

### Quick Start with Docker (Recommended)

1. **Start the entire stack** (API + PostgreSQL):
   ```bash
   docker-compose up -d
   ```

2. **Access the services**:
   - API: http://localhost:8081
   - Swagger UI: http://localhost:8081/swagger
   - PostgreSQL: localhost:5433 (user: postgres, password: postgres, db: driver_services)

3. **View logs**:
   ```bash
   docker-compose logs -f driver-services-api
   ```

4. **Stop all services**:
   ```bash
   docker-compose down
   ```

5. **Reset database** (removes all data):
   ```bash
   docker-compose down -v
   docker-compose up -d
   ```

### Local Development Setup

1. **Start PostgreSQL Database**:
   ```bash
   docker-compose up -d driver-services-db
   ```

2. **Configure connection string** in `appsettings.Development.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5433;Database=driver_services;Username=postgres;Password=postgres"
   }
   ```

3. **Run the API locally**:
   ```bash
   cd Driver.Services.Api
   dotnet run
   ```

4. **Access Swagger UI**:
   Navigate to `http://localhost:5001/swagger`

### Configuration

#### Environment Variables

The following environment variables can be configured in `docker-compose.yml`:

**PostgreSQL**:
- `POSTGRES_USER` - Database user (default: postgres)
- `POSTGRES_PASSWORD` - Database password (default: postgres)
- `POSTGRES_DB` - Database name (default: driver_services)

**API**:
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string

#### Configuration Files

Configuration is stored in `appsettings.json` and `appsettings.Development.json`:

- **ConnectionStrings:DefaultConnection**: PostgreSQL connection string
- **JwtSettings**: JWT token configuration (SecretKey, Issuer, Audience, ExpirationMinutes)

For production deployments, use environment variables or secrets management instead of `appsettings.json`.

### Development

Build the entire solution:
```bash
dotnet build
```

Run tests:
```bash
dotnet test
```

#### Docker Development Workflow

**Rebuild after code changes**:
```bash
docker-compose up -d --build
```

**Access database directly**:
```bash
docker exec -it driver-services-db psql -U postgres -d driver_services
```

**View container status**:
```bash
docker-compose ps
```

**Clean rebuild** (removes old images):
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

## Project Structure

```
Driver.Services/
├── Driver.Services.Api/          # Web API layer
│   ├── Endpoints/                # Endpoint groups
│   ├── Identity/                 # Authentication/Authorization
│   └── Program.cs                # Entry point
├── Driver.Services.Application/  # Application layer
│   ├── Common/                   # Shared application logic
│   │   ├── Behaviours/          # MediatR pipeline behaviors
│   │   ├── Interfaces/          # Application interfaces
│   │   └── Models/              # DTOs and view models
│   └── UseCases/                # CQRS commands and queries
├── Driver.Services.Infrastructure/ # Infrastructure layer
│   ├── Data/                    # EF Core DbContext and configurations
│   ├── Repositories/            # Repository implementations
│   └── DependencyInjection.cs   # Infrastructure DI registration
└── Driver.Services.Domain/       # Domain layer
    ├── Abstractions/            # Base classes and interfaces
    ├── AggregatesModel/         # Domain aggregates
    │   ├── DriverAggregate/
    │   ├── DriverLocationAggregate/
    │   ├── DriverWalletAggregate/
    │   └── TripHistoryAggregate/
    ├── Constants/               # Domain constants
    ├── Events/                  # Domain events
    └── Exceptions/              # Domain exceptions
```

## API Endpoints

### Driver Management
- `POST /api/drivers/register` - Register a new driver
- `PUT /api/drivers/{id}/verify` - Verify driver documents
- `GET /api/drivers/{id}` - Get driver profile
- `PUT /api/drivers/{id}/vehicle` - Update vehicle information

### Status & Location
- `PUT /api/drivers/{id}/status` - Update driver status (Online/Offline/Busy)
- `POST /api/drivers/{id}/location` - Update driver location
- `GET /api/drivers/nearby` - Get nearby available drivers

### Orders & Trips
- `POST /api/orders/dispatch` - Dispatch order to driver
- `GET /api/drivers/{id}/trips` - Get driver trip history
- `GET /api/drivers/{id}/earnings` - Get driver earnings

### Wallet
- `GET /api/drivers/{id}/wallet` - Get wallet details
- `POST /api/drivers/{id}/wallet/deposit` - Record COD deposit
- `POST /api/drivers/{id}/wallet/withdraw` - Process withdrawal

## Authentication

The API uses JWT Bearer authentication with role-based authorization:

- **Driver**: Access to own profile and operations
- **Dispatcher**: Assign orders to drivers
- **Admin**: Full system access

Include JWT token in request headers:
```
Authorization: Bearer {your-token}
```

## Database Schema

### Core Tables
- **Drivers**: Driver profiles and verification status
- **DriverLocations**: Real-time GPS tracking
- **DriverWallets**: Financial operations and balances
- **TripHistory**: Order tracking and earnings

## Development Roadmap

- [x] Phase 1: Project setup and infrastructure
- [ ] Phase 2: Domain layer implementation
- [ ] Phase 3: Application layer (use cases)
- [ ] Phase 4: Infrastructure layer (data access)
- [ ] Phase 5: API layer (endpoints)
- [ ] Phase 6: Testing and documentation

## License

Proprietary - All rights reserved
