# Quick Start Guide - Restaurant Service

## Prerequisites Check
- [ ] Python 3.9+ installed
- [ ] PostgreSQL installed and running
- [ ] Database created

## Step 1: Install Dependencies

```bash
# Activate virtual environment (create one if you haven't)
python -m venv venv
venv\Scripts\activate  # Windows
# source venv/bin/activate  # Linux/Mac

# Install packages
pip install -r requirements.txt
```

## Step 2: Configure Environment

```bash
# Copy the example environment file
copy .env.example .env  # Windows
# cp .env.example .env  # Linux/Mac

# Edit .env and set your database URL
# DATABASE_URL=postgresql://username:password@localhost:5432/restaurant_db
```

## Step 3: Set Up Database

```bash
# Make sure PostgreSQL is running
# Create the database if it doesn't exist:
# psql -U postgres -c "CREATE DATABASE restaurant_db;"

# Run migrations
alembic upgrade head
```

If you need to generate the migration first:
```bash
alembic revision --autogenerate -m "create restaurants table"
alembic upgrade head
```

## Step 4: Run the Application

```bash
uvicorn main:app --reload
```

You should see:
```
INFO:     Uvicorn running on http://127.0.0.1:8000 (Press CTRL+C to quit)
INFO:     Started reloader process
INFO:     Started server process
INFO:     Waiting for application startup.
INFO:     Application startup complete.
```

## Step 5: Test the API

Open your browser and visit:
- **API Docs**: http://localhost:8000/docs
- **Health Check**: http://localhost:8000/health

Or use curl:
```bash
# Health check
curl http://localhost:8000/health

# Create a restaurant
curl -X POST "http://localhost:8000/api/v1/restaurants/" \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Test Restaurant\",\"address\":\"123 Test St\",\"phone\":\"0123456789\"}"

# List restaurants
curl http://localhost:8000/api/v1/restaurants/
```

## Troubleshooting

### Issue: "No module named 'app'"
**Solution**: Make sure you're in the project root directory (backend-grab/)

### Issue: Database connection errors
**Solution**: 
1. Verify PostgreSQL is running
2. Check DATABASE_URL in .env file
3. Verify database exists: `psql -U postgres -l`

### Issue: Alembic errors
**Solution**:
1. Check alembic/env.py has the correct imports
2. Make sure all models are imported in alembic/env.py
3. Try: `alembic revision --autogenerate -m "init"` then `alembic upgrade head`

### Issue: Import errors
**Solution**: Verify your project structure matches the README

## Next Steps

1. Create a Users table for authentication
2. Add authentication middleware
3. Add authorization checks in endpoints
4. Create MenuItem and Order models
5. Add tests

## Common Commands

```bash
# Create new migration
alembic revision --autogenerate -m "description"

# Apply migrations
alembic upgrade head

# Rollback one migration
alembic downgrade -1

# Run server with auto-reload
uvicorn main:app --reload

# Run server on different port
uvicorn main:app --port 8001

# Run in production mode (no reload)
uvicorn main:app --host 0.0.0.0 --port 8000
```
