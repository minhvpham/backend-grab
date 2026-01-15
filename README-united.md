# United Services Docker Compose

This docker-compose file runs both Order.Services and Driver.Services on the same PostgreSQL database container.

## Usage

1. Start the services:
   docker-compose -f docker-compose.united.yml up -d

2. Check logs:
   docker-compose -f docker-compose.united.yml logs

3. Stop the services:
   docker-compose -f docker-compose.united.yml down

## Services

- **postgres**: Shared PostgreSQL database (port 5432)
  - Database: order_service_db (for Order.Service)
  - Database: driver_services (for Driver.Service)
  - User: postgres
  - Password: united_password

- **order-service**: Order Service API (port 8002)
- **driver-service**: Driver Service API (port 8081)

## Databases

- order_service_db: Contains users, orders, order_items tables
- driver_services: Contains Drivers, DriverLocations, DriverWallets, Transactions, TripHistories tables

Both services connect to the same PostgreSQL container but use different databases.
