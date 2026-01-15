# United Services Docker Compose

This docker-compose file runs both Order.Services and Driver.Services on the **same PostgreSQL database** (single shared database).

## Usage

1. Start the services:
   docker-compose -f docker-compose.united.yml up -d

2. Check logs:
   docker-compose -f docker-compose.united.yml logs

3. Stop the services:
   docker-compose -f docker-compose.united.yml down

## Services

- **postgres**: Shared PostgreSQL database (port 5432)
  - Database: united_services (single shared database)
  - User: postgres
  - Password: united_password

- **order-service**: Order Service API (port 8002)
- **driver-service**: Driver Service API (port 8081)

## Shared Database Schema

The `united_services` database contains all tables from both services:

### Order Service Tables (lowercase):
- `users` - User accounts
- `orders` - Order records
- `order_items` - Order line items

### Driver Service Tables (PascalCase with quotes):
- `"Drivers"` - Driver profiles
- `"DriverLocations"` - Driver GPS locations
- `"DriverWallets"` - Driver payment wallets
- `"Transactions"` - Wallet transactions
- `"TripHistories"` - Trip records
- `"__EFMigrationsHistory"` - Entity Framework migrations

Both services connect to the same PostgreSQL container and use the same `united_services` database, allowing for potential cross-service queries and data relationships.
