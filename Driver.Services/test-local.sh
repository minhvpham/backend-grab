#!/bin/bash

# Test script for Driver Services API (Local Development)
# This script tests the API without Docker

set -e

echo "=== Driver Services Local Test ==="
echo ""

# Check if PostgreSQL is running
echo "1. Checking PostgreSQL connection..."
if docker ps | grep -q "driver-services-db"; then
    echo "   ✓ PostgreSQL container is running"
else
    echo "   ✗ PostgreSQL container not found"
    echo "   Starting PostgreSQL..."
    docker-compose up -d db
    echo "   Waiting for PostgreSQL to be ready..."
    sleep 5
fi

# Test database connection
echo ""
echo "2. Testing database connection..."
if docker exec driver-services-db pg_isready -U postgres > /dev/null 2>&1; then
    echo "   ✓ Database is ready"
else
    echo "   ✗ Database not ready, waiting..."
    sleep 5
fi

# Check if dotnet-ef is installed
if ! command -v dotnet-ef &> /dev/null; then
    echo ""
    echo "3. Installing dotnet-ef tool..."
    dotnet tool install --global dotnet-ef --version 8.0.11
fi

# Apply migrations
echo ""
echo "4. Applying database migrations..."
cd /home/bojack/Code/backend-grab/Driver.Services/Driver.Services.Infrastructure
export PATH="$PATH:$HOME/.dotnet/tools"
dotnet ef database update --startup-project ../Driver.Services.Api

echo ""
echo "5. Building the application..."
cd /home/bojack/Code/backend-grab/Driver.Services
dotnet build --configuration Release

echo ""
echo "6. Application is ready!"
echo ""
echo "To run the API:"
echo "  cd Driver.Services.Api"
echo "  dotnet run"
echo ""
echo "API will be available at:"
echo "  - http://localhost:5000"
echo "  - https://localhost:5001"
echo "  - Swagger UI: http://localhost:5000/swagger"
echo ""
echo "Database connection:"
echo "  - Host: localhost:5432"
echo "  - Database: driver_services_dev"
echo "  - User: postgres"
echo "  - Password: 1"
echo ""
