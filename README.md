# Restaurant Service Backend

A FastAPI-based backend service for managing restaurants in a food delivery system (Grab-like application).

## Features

- **Restaurant Management**: Create, Read, Update, Delete (CRUD) operations
- **Restaurant Status**: Support for approval workflow (PENDING, ACTIVE, BANNED, REJECTED)
- **Owner Management**: Link restaurants to user owners
- **Location Support**: Store latitude/longitude for mapping
- **Operating Hours**: Track restaurant open/closed status
- **Rating System**: Store restaurant ratings

## Project Structure

```
backend-grab/
├── alembic/                    # Database migrations
│   ├── versions/              # Migration files
│   └── env.py                 # Alembic configuration
├── app/
│   ├── api/
│   │   ├── endpoints/
│   │   │   └── restaurants.py # Restaurant API routes
│   │   └── __init__.py
│   ├── crud/
│   │   ├── restaurant.py      # Database operations
│   │   └── __init__.py
│   ├── db/
│   │   └── base.py            # Database connection & session
│   ├── models/
│   │   ├── restaurant.py      # SQLAlchemy models
│   │   └── __init__.py
│   └── schemas/
│       ├── restaurant.py      # Pydantic schemas
│       └── __init__.py
├── main.py                    # FastAPI application entry point
├── requirements.txt           # Python dependencies
├── alembic.ini               # Alembic configuration
└── .env.example              # Environment variables template
```

## Installation

### Prerequisites

- Python 3.9+
- PostgreSQL 12+
- pip

### Setup Steps

1. **Clone the repository** (if applicable)

2. **Create a virtual environment**:
   ```bash
   python -m venv venv
   ```

3. **Activate the virtual environment**:
   - Windows:
     ```bash
     venv\Scripts\activate
     ```
   - Linux/Mac:
     ```bash
     source venv/bin/activate
     ```

4. **Install dependencies**:
   ```bash
   pip install -r requirements.txt
   ```

5. **Set up environment variables**:
   - Copy `.env.example` to `.env`
   - Edit `.env` and set your PostgreSQL connection string:
     ```
     DATABASE_URL=postgresql://username:password@localhost:5432/restaurant_db
     ```

6. **Create the database** (if it doesn't exist):
   ```sql
   CREATE DATABASE restaurant_db;
   ```

7. **Run database migrations**:
   ```bash
   alembic upgrade head
   ```
   
   Or generate a new migration if needed:
   ```bash
   alembic revision --autogenerate -m "create restaurants table"
   alembic upgrade head
   ```

8. **Run the application**:
   ```bash
   uvicorn main:app --reload
   ```

The API will be available at `http://localhost:8000`

## API Documentation

Once the server is running, visit:
- **Swagger UI**: http://localhost:8000/docs
- **ReDoc**: http://localhost:8000/redoc

## API Endpoints

### Restaurant Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/restaurants/` | Create a new restaurant |
| GET | `/api/v1/restaurants/` | List all restaurants (with filters) |
| GET | `/api/v1/restaurants/{id}` | Get a specific restaurant |
| GET | `/api/v1/restaurants/owner/{owner_id}` | Get restaurants by owner |
| PUT | `/api/v1/restaurants/{id}` | Update restaurant details |
| PATCH | `/api/v1/restaurants/{id}/toggle-open` | Toggle open/closed status |
| PATCH | `/api/v1/restaurants/{id}/status` | Update restaurant status (Admin) |
| DELETE | `/api/v1/restaurants/{id}` | Delete a restaurant |

### Example Requests

**Create a Restaurant:**

*Windows PowerShell:*
```powershell
Invoke-RestMethod -Uri "http://localhost:8000/api/v1/restaurants/" -Method Post -ContentType "application/json" -Body (@{
    name = "Pho 24"
    description = "Authentic Vietnamese noodle soup"
    address = "123 Main St, District 1, HCMC"
    phone = "0901234567"
    latitude = 10.7769
    longitude = 106.7009
    opening_hours = "08:00-22:00"
} | ConvertTo-Json)
```

*Windows CMD (curl):*
```cmd
curl -X POST "http://localhost:8000/api/v1/restaurants/" -H "Content-Type: application/json" -d "{\"name\":\"Pho 24\",\"description\":\"Authentic Vietnamese noodle soup\",\"address\":\"123 Main St, District 1, HCMC\",\"phone\":\"0901234567\",\"latitude\":10.7769,\"longitude\":106.7009,\"opening_hours\":\"08:00-22:00\"}"
```

*Linux/Mac:*
```bash
curl -X POST "http://localhost:8000/api/v1/restaurants/" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Pho 24",
    "description": "Authentic Vietnamese noodle soup",
    "address": "123 Main St, District 1, HCMC",
    "phone": "0901234567",
    "latitude": 10.7769,
    "longitude": 106.7009,
    "opening_hours": "08:00-22:00"
  }'
```

**Get All Restaurants:**
```bash
curl "http://localhost:8000/api/v1/restaurants/"
```

**Get Restaurant by ID:**
```bash
curl "http://localhost:8000/api/v1/restaurants/5"
```
*Note: Replace `5` with the actual restaurant ID from the create response*

**Update Restaurant:**

*Windows PowerShell:*
```powershell
Invoke-RestMethod -Uri "http://localhost:8000/api/v1/restaurants/5" -Method Put -ContentType "application/json" -Body (@{
    name = "Pho 24 - Updated"
    is_open = $false
} | ConvertTo-Json)
```

*Windows CMD (curl):*
```cmd
curl -X PUT "http://localhost:8000/api/v1/restaurants/5" -H "Content-Type: application/json" -d "{\"name\":\"Pho 24 - Updated\",\"is_open\":false}"
```
*Note: Replace `5` with your restaurant ID*

**Update Status (Admin):**
```bash
curl -X PATCH "http://localhost:8000/api/v1/restaurants/5/status?new_status=ACTIVE"
```

## Database Schema

### Restaurant Table

| Column | Type | Description |
|--------|------|-------------|
| id | INTEGER | Primary key |
| owner_id | INTEGER | Foreign key to users table |
| name | VARCHAR(100) | Restaurant name |
| description | TEXT | Restaurant description |
| address | TEXT | Physical address |
| phone | VARCHAR(20) | Contact phone |
| latitude | NUMERIC(10,8) | GPS latitude |
| longitude | NUMERIC(11,8) | GPS longitude |
| opening_hours | VARCHAR(100) | Operating hours |
| is_open | BOOLEAN | Current open/closed status |
| rating | NUMERIC(2,1) | Rating (0.0-5.0) |
| status | ENUM | PENDING/ACTIVE/BANNED/REJECTED |
| created_at | TIMESTAMP | Creation timestamp |

## Restaurant Status Flow

```
PENDING → ACTIVE (Admin approves)
PENDING → REJECTED (Admin rejects)
ACTIVE → BANNED (Admin bans)
```

## TODO / Future Enhancements

- [ ] Add user authentication (JWT tokens)
- [ ] Add authorization checks (owner vs admin)
- [ ] Create Users table and model
- [ ] Add MenuItem management
- [ ] Add Order management
- [ ] Implement search and filtering (by location, rating, cuisine)
- [ ] Add pagination helpers
- [ ] Add restaurant image upload
- [ ] Add operating hours validation
- [ ] Add soft delete instead of hard delete
- [ ] Add audit logging
- [ ] Add unit tests
- [ ] Add integration tests

## Development

### Running Tests
```bash
# TODO: Add pytest configuration
pytest
```

### Creating New Migrations
```bash
alembic revision --autogenerate -m "your migration message"
alembic upgrade head
```

### Downgrading Migrations
```bash
alembic downgrade -1  # Go back one migration
alembic downgrade base  # Go back to beginning
```

## Tech Stack

- **Framework**: FastAPI 0.109.0
- **Database**: PostgreSQL with SQLAlchemy 2.0
- **Migrations**: Alembic 1.13.1
- **Validation**: Pydantic 2.5.3
- **Server**: Uvicorn

## License

[Add your license here]

## Contributors

[Add contributors here]
