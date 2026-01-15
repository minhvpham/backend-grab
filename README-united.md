# United Services Docker Compose

Runs Order, Driver, and Auth services on the same PostgreSQL database container.

## Services

### PostgreSQL (port 5432)
- **order_service_db**: Order Service database (users, orders, order_items tables)
- **driver_services**: Driver Service database (Drivers, DriverLocations, DriverWallets, Transactions, TripHistories tables)
- **auth_services**: Auth Service database (users table with authentication)
- User: postgres
- Password: united_password

### APIs
- **order-service**: Order Service API (port 8002)
- **driver-service**: Driver Service API (port 8081)
- **auth-service**: Auth Service API (port 8003)

## Quick Start

```bash
# Start all services
docker-compose up -d

# Check service health
docker-compose ps

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: deletes all data)
docker-compose down -v
```

## Database Access

```bash
# Access PostgreSQL
docker exec -it united-services-postgres psql -U postgres

# Connect to specific database
docker exec -it united-services-postgres psql -U postgres -d auth_services

# List all databases
docker exec -it united-services-postgres psql -U postgres -c "\l"
```

## Service Endpoints

- Order Service: http://localhost:8002
- Driver Service: http://localhost:8081
- Auth Service: http://localhost:8003

## Database Schemas

### Order Service (order_service_db)
- users: User information
- orders: Order details
- order_items: Order line items

### Driver Service (driver_services)
- Drivers: Driver profiles
- DriverLocations: GPS locations
- DriverWallets: Financial data
- Transactions: Wallet transactions
- TripHistories: Trip records

### Auth Service (auth_services)
- users: Authentication users with roles and status
