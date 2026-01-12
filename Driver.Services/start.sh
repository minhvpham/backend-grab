#!/bin/bash

echo "🚀 Starting Driver Services..."
echo ""

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Error: Docker is not running. Please start Docker and try again."
    exit 1
fi

# Build and start services
echo "📦 Building and starting services..."
docker-compose up --build -d

# Wait for services to be healthy
echo ""
echo "⏳ Waiting for services to be ready..."
sleep 5

# Check if services are running
if docker-compose ps | grep -q "driver-services-api.*Up"; then
    echo ""
    echo "✅ Driver Services API is running!"
    echo ""
    echo "📍 API Endpoints:"
    echo "   - API Base URL:    http://localhost:8080"
    echo "   - Swagger UI:      http://localhost:8080/swagger"
    echo "   - Health Check:    http://localhost:8080/health"
    echo ""
    echo "📊 Database:"
    echo "   - Host:            localhost:5432"
    echo "   - Database:        driver_services"
    echo "   - Username:        postgres"
    echo "   - Password:        postgres"
    echo ""
    echo "📋 Useful Commands:"
    echo "   - View logs:       docker-compose logs -f api"
    echo "   - Stop services:   docker-compose down"
    echo "   - Restart API:     docker-compose restart api"
    echo "   - Clean reset:     docker-compose down -v"
    echo ""
else
    echo "❌ Error: API failed to start. Check logs with: docker-compose logs api"
    exit 1
fi
