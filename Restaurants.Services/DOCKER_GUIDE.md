# Docker Setup Guide

## Prerequisites
- Docker Desktop installed and running
- Docker Compose installed (included with Docker Desktop)

## Quick Start

### 1. Build and Start the Services
```powershell
docker-compose up --build
```

Or run in detached mode (background):
```powershell
docker-compose up -d --build
```

### 2. Access the Application
- API: http://localhost:8000
- API Documentation: http://localhost:8000/docs
- Health Check: http://localhost:8000/health

### 3. Run Database Migrations
Migrations run automatically on startup, but you can run them manually:
```powershell
docker-compose exec backend alembic upgrade head
```

### 4. Seed the Database (Optional)
```powershell
docker-compose exec backend python seed_data.py
```

## Common Docker Commands

### View Running Containers
```powershell
docker-compose ps
```

### View Logs
```powershell
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend
docker-compose logs -f db
```

### Stop Services
```powershell
docker-compose stop
```

### Start Services (after stop)
```powershell
docker-compose start
```

### Restart Services
```powershell
docker-compose restart
```

### Stop and Remove Containers
```powershell
docker-compose down
```

### Stop and Remove Containers + Volumes (⚠️ deletes data)
```powershell
docker-compose down -v
```

### Rebuild Containers
```powershell
docker-compose up --build
```

### Execute Commands in Running Container
```powershell
# Open bash shell
docker-compose exec backend bash

# Run Python script
docker-compose exec backend python seed_data.py

# Create new migration
docker-compose exec backend alembic revision --autogenerate -m "description"
```

## Database Access

### Connect to PostgreSQL
```powershell
docker-compose exec db psql -U grab_user -d grab_restaurant_db
```

Common PostgreSQL commands:
```sql
-- List tables
\dt

-- Describe table
\d restaurants

-- Query data
SELECT * FROM restaurants;

-- Exit
\q
```

## Environment Variables

The default configuration uses:
- **Database User**: grab_user
- **Database Password**: grab_password
- **Database Name**: grab_restaurant_db
- **Database Port**: 5432
- **Backend Port**: 8000

To customize, create a `.env` file (see `.env.example`).

## Troubleshooting

### Port Already in Use
If port 5432 or 8000 is already in use, modify the ports in `docker-compose.yml`:
```yaml
ports:
  - "5433:5432"  # Use 5433 on host instead of 5432
```

### Database Connection Issues
1. Check if database is healthy:
   ```powershell
   docker-compose ps
   ```
2. View database logs:
   ```powershell
   docker-compose logs db
   ```
3. Restart services:
   ```powershell
   docker-compose restart
   ```

### Reset Everything
```powershell
# Stop and remove all containers, networks, and volumes
docker-compose down -v

# Rebuild and start
docker-compose up --build
```

### View Container Resource Usage
```powershell
docker stats
```

## Production Considerations

For production deployment, modify `docker-compose.yml`:

1. Remove `--reload` flag from uvicorn
2. Use production-grade database password
3. Add environment-specific configuration
4. Consider using separate docker-compose.prod.yml
5. Enable HTTPS/SSL
6. Configure proper logging
7. Set up health monitoring

## Docker Compose File Structure

```
services:
  db:          # PostgreSQL database
  backend:     # FastAPI application
```

Both services are connected via the `grab_network` bridge network, allowing them to communicate using service names as hostnames.
