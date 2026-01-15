# Restaurant Service Backend

A FastAPI-based backend service for managing restaurants in a food delivery system (Grab-like application).

## Features

- **Restaurant Management**: Create, Read, Update, Delete (CRUD) operations
- **Restaurant Status**: Support for approval workflow (PENDING, ACTIVE, BANNED, REJECTED)
- **Owner Management**: Link restaurants to user owners
- **Document Upload**: Business license and food safety certificate image storage
- **Operating Hours**: Track restaurant open/closed status
- **Rating System**: Store restaurant ratings
- **Menu Management**: Categories and menu items (dishes) with availability tracking
- **Analytics**: Dish sales statistics (requires Order module)

## Project Structure

```
backend-grab/
├── alembic/                    # Database migrations
│   ├── versions/              # Migration files
│   └── env.py                 # Alembic configuration
├── app/
│   ├── api/
│   │   ├── endpoints/
│   │   │   ├── restaurants.py # Restaurant API routes
│   │   │   └── menu.py        # Menu & Category API routes
│   │   └── __init__.py
│   ├── crud/
│   │   ├── restaurant.py      # Restaurant database operations
│   │   ├── menu.py            # Menu database operations
│   │   └── __init__.py
│   ├── db/
│   │   └── base.py            # Database connection & session
│   ├── models/
│   │   ├── restaurant.py      # Restaurant SQLAlchemy model
│   │   ├── menu.py            # Category & MenuItem models
│   │   ├── auth.py            # User model
│   │   └── __init__.py
│   ├── schemas/
│   │   ├── restaurant.py      # Restaurant Pydantic schemas
│   │   ├── menu.py            # Menu Pydantic schemas
│   │   └── __init__.py
│   └── utils/
│       ├── file_upload.py     # Image upload utilities
│       └── __init__.py
├── uploads/                   # Image storage directory
│   ├── business_licenses/     # Business license images
│   └── food_safety_certificates/ # Food safety cert images
├── main.py                    # FastAPI application entry point
├── requirements.txt           # Python dependencies
├── alembic.ini               # Alembic configuration
└── .env.example              # Environment variables template
```

## Installation

### Option 1: Docker Compose (Recommended)

This is the easiest way to get started - Docker Compose will set up both PostgreSQL and the backend application.

#### Prerequisites
- Docker Desktop (Windows/Mac) or Docker Engine + Docker Compose (Linux)

#### Steps

1. **Start the services**:
   ```bash
   docker-compose up -d
   ```
   
   This will:
   - Pull and start PostgreSQL 15 database
   - Build the FastAPI backend image
   - Run database migrations automatically
   - Start the application on port 8080

2. **Check the logs**:
   ```bash
   docker-compose logs -f backend
   ```

3. **Access the application**:
   - API: http://localhost:8080
   - Swagger UI: http://localhost:8080/docs
   - ReDoc: http://localhost:8080/redoc

4. **Stop the services**:
   ```bash
   docker-compose down
   ```

5. **Stop and remove volumes** (removes database data):
   ```bash
   docker-compose down -v
   ```

#### Useful Docker Commands

```bash
# View running containers
docker-compose ps

# View logs for all services
docker-compose logs

# Rebuild the backend after code changes
docker-compose up -d --build backend

# Access the backend container shell
docker-compose exec backend bash

# Access PostgreSQL shell
docker-compose exec db psql -U grab_user -d grab_restaurant_db

# Run Alembic migrations manually
docker-compose exec backend alembic upgrade head

# Create a new migration
docker-compose exec backend alembic revision --autogenerate -m "your message"
```

### Option 2: Local Installation

#### Prerequisites

- Python 3.9+
- PostgreSQL 12+
- pip

#### Setup Steps

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

---

## Quick Start with Docker

For the fastest setup, use Docker Compose:

```bash
# Start everything (database + backend)
docker-compose up -d

# Check logs
docker-compose logs -f

# Access the API
# - Swagger UI: http://localhost:8080/docs
# - API: http://localhost:8080
```

The Docker setup includes:
- **PostgreSQL 15** database on port 5432
- **FastAPI backend** on port 8080
- **Automatic migrations** on startup
- **Hot reload** for development
- **Persistent data** in Docker volumes

## API Documentation

Once the server is running, visit:
- **Swagger UI**: http://localhost:8000/docs
- **ReDoc**: http://localhost:8000/redoc

## API Endpoints

### Restaurant APIs

#### 1. Create Restaurant
**POST** `/api/v1/restaurants/`

- **What it does**: Creates a new restaurant with business license and food safety certificate images
- **Input**: Multipart form data with:
  - `owner_id` (integer, required): User ID of the restaurant owner
  - `name` (string, required): Restaurant name
  - `address` (string, required): Physical address
  - `phone` (string, optional): Contact phone number
  - `description` (string, optional): Restaurant description
  - `opening_hours` (string, optional): Operating hours (e.g., "08:00-22:00")
  - `business_license_image` (file, required): Business license image (.jpg, .jpeg, .png, .gif, .bmp, .webp)
  - `food_safety_certificate_image` (file, required): Food safety certificate image
- **Output**: Restaurant object with:
  - `id`: Restaurant ID
  - `owner_id`: Owner user ID
  - `name`: Restaurant name
  - `description`: Restaurant description
  - `address`: Physical address
  - `phone`: Contact phone
  - `opening_hours`: Operating hours
  - `business_license_image`: Stored image path
  - `food_safety_certificate_image`: Stored image path
  - `is_open`: Current open/closed status (default: true)
  - `rating`: Rating 0.0-5.0 (default: 0.0)
  - `status`: PENDING, ACTIVE, BANNED, or REJECTED (default: PENDING)
  - `created_at`: Creation timestamp

#### 2. List Restaurants
**GET** `/api/v1/restaurants/`

- **What it does**: Retrieves a paginated list of restaurants with optional filtering
- **Input**: Query parameters:
  - `skip` (integer, optional, default: 0): Number of records to skip for pagination
  - `limit` (integer, optional, default: 100, max: 100): Maximum number of records to return
  - `status` (string, optional): Filter by status (PENDING, ACTIVE, BANNED, REJECTED)
- **Output**: Array of restaurant objects (same structure as Create Restaurant output)

#### 3. Get Restaurant by ID
**GET** `/api/v1/restaurants/{restaurant_id}`

- **What it does**: Retrieves details of a specific restaurant
- **Input**: Path parameter:
  - `restaurant_id` (integer, required): Restaurant ID
- **Output**: Restaurant object (same structure as Create Restaurant output)

#### 4. List Restaurants by Owner
**GET** `/api/v1/restaurants/owner/{owner_id}`

- **What it does**: Gets all restaurants owned by a specific user
- **Input**: Path parameter:
  - `owner_id` (integer, required): User ID of the restaurant owner
- **Output**: Array of restaurant objects (same structure as Create Restaurant output)

#### 5. Update Restaurant
**PUT** `/api/v1/restaurants/{restaurant_id}`

- **What it does**: Updates restaurant information (owner only)
- **Input**:
  - Path parameter: `restaurant_id` (integer, required)
  - Request body (JSON) with optional fields:
    - `name` (string): Restaurant name
    - `description` (string): Restaurant description
    - `address` (string): Physical address
    - `phone` (string): Contact phone
    - `opening_hours` (string): Operating hours
    - `business_license_image` (string): Business license image path
    - `food_safety_certificate_image` (string): Food safety certificate image path
    - `is_open` (boolean): Open/closed status
    - `rating` (number): Rating 0.0-5.0
- **Output**: Updated restaurant object

#### 6. Toggle Restaurant Open/Closed Status
**PATCH** `/api/v1/restaurants/{restaurant_id}/toggle-open`

- **What it does**: Toggles the restaurant's open/closed status (owner only)
- **Input**: Path parameter:
  - `restaurant_id` (integer, required): Restaurant ID
- **Output**: Updated restaurant object with toggled `is_open` status

#### 7. Update Restaurant Status (Admin)
**PATCH** `/api/v1/restaurants/{restaurant_id}/status`

- **What it does**: Updates restaurant approval status (admin only)
- **Input**:
  - Path parameter: `restaurant_id` (integer, required)
  - Query parameter: `new_status` (string, required): PENDING, ACTIVE, BANNED, or REJECTED
- **Output**: Updated restaurant object

#### 8. Delete Restaurant
**DELETE** `/api/v1/restaurants/{restaurant_id}`

- **What it does**: Permanently deletes a restaurant (owner/admin only)
- **Input**: Path parameter:
  - `restaurant_id` (integer, required): Restaurant ID
- **Output**: HTTP 204 No Content on success

---

### Category APIs

#### 1. Create Category
**POST** `/api/v1/menu/restaurants/{restaurant_id}/categories`

- **What it does**: Creates a new category for organizing menu items
- **Input**:
  - Path parameter: `restaurant_id` (integer, required)
  - Request body (JSON):
    - `name` (string, required): Category name (e.g., "Appetizers", "Main Courses")
    - `description` (string, optional): Category description
    - `display_order` (integer, optional): Order for displaying categories
- **Output**: Category object with:
  - `id`: Category ID
  - `restaurant_id`: Restaurant ID
  - `name`: Category name
  - `description`: Category description
  - `display_order`: Display order
  - `created_at`: Creation timestamp

#### 2. List Categories by Restaurant
**GET** `/api/v1/menu/restaurants/{restaurant_id}/categories`

- **What it does**: Gets all categories for a specific restaurant
- **Input**: Path parameter:
  - `restaurant_id` (integer, required): Restaurant ID
- **Output**: Array of category objects

#### 3. Get Category by ID
**GET** `/api/v1/menu/categories/{category_id}`

- **What it does**: Retrieves details of a specific category
- **Input**: Path parameter:
  - `category_id` (integer, required): Category ID
- **Output**: Category object

#### 4. Update Category
**PUT** `/api/v1/menu/categories/{category_id}`

- **What it does**: Updates category information
- **Input**:
  - Path parameter: `category_id` (integer, required)
  - Request body (JSON) with optional fields:
    - `name` (string): Category name
    - `description` (string): Category description
    - `display_order` (integer): Display order
- **Output**: Updated category object

#### 5. Delete Category
**DELETE** `/api/v1/menu/categories/{category_id}`

- **What it does**: Deletes a category (affects linked menu items)
- **Input**: Path parameter:
  - `category_id` (integer, required): Category ID
- **Output**: HTTP 204 No Content on success

---

### Menu Item (Dish) APIs

#### 1. Create Dish
**POST** `/api/v1/menu/restaurants/{restaurant_id}/dishes`

- **What it does**: Creates a new menu item (dish) for a restaurant
- **Input**:
  - Path parameter: `restaurant_id` (integer, required)
  - Request body (JSON):
    - `name` (string, required): Dish name
    - `description` (string, optional): Dish description
    - `price` (number, required): Price in your currency
    - `category_id` (integer, optional): Category ID to assign dish
    - `image_url` (string, optional): Image URL
    - `is_available` (boolean, optional, default: true): Stock availability
    - `stock_quantity` (integer, optional): Stock quantity
- **Output**: Menu item object with:
  - `id`: Menu item ID
  - `restaurant_id`: Restaurant ID
  - `category_id`: Category ID (if assigned)
  - `name`: Dish name
  - `description`: Dish description
  - `price`: Price
  - `image_url`: Image URL
  - `is_available`: Availability status
  - `stock_quantity`: Stock quantity
  - `created_at`: Creation timestamp
  - `updated_at`: Last update timestamp

#### 2. List Dishes by Restaurant
**GET** `/api/v1/menu/restaurants/{restaurant_id}/dishes`

- **What it does**: Gets all menu items for a restaurant with optional filters
- **Input**:
  - Path parameter: `restaurant_id` (integer, required)
  - Query parameters:
    - `category_id` (integer, optional): Filter by category
    - `available_only` (boolean, optional, default: false): Show only available items
- **Output**: Array of menu item objects

#### 3. List Dishes by Category
**GET** `/api/v1/menu/categories/{category_id}/dishes`

- **What it does**: Gets all menu items in a specific category
- **Input**: Path parameter:
  - `category_id` (integer, required): Category ID
- **Output**: Array of menu item objects

#### 4. Get Dish by ID
**GET** `/api/v1/menu/dishes/{dish_id}`

- **What it does**: Retrieves details of a specific dish
- **Input**: Path parameter:
  - `dish_id` (integer, required): Dish ID
- **Output**: Menu item object

#### 5. Update Dish
**PUT** `/api/v1/menu/dishes/{dish_id}`

- **What it does**: Updates dish information (name, price, availability, etc.)
- **Input**:
  - Path parameter: `dish_id` (integer, required)
  - Request body (JSON) with optional fields:
    - `name` (string): Dish name
    - `description` (string): Dish description
    - `price` (number): Price
    - `category_id` (integer): Category ID
    - `image_url` (string): Image URL
    - `is_available` (boolean): Availability status
    - `stock_quantity` (integer): Stock quantity
- **Output**: Updated menu item object

#### 6. Toggle Dish Availability
**PATCH** `/api/v1/menu/dishes/{dish_id}/toggle-availability`

- **What it does**: Quickly toggles dish availability (in stock / out of stock)
- **Input**: Path parameter:
  - `dish_id` (integer, required): Dish ID
- **Output**: Updated menu item object with toggled `is_available` status

#### 7. Delete Dish
**DELETE** `/api/v1/menu/dishes/{dish_id}`

- **What it does**: Permanently deletes a menu item
- **Input**: Path parameter:
  - `dish_id` (integer, required): Dish ID
- **Output**: HTTP 204 No Content on success

#### 8. Get Dish Statistics
**GET** `/api/v1/menu/dishes/{dish_id}/stats`

- **What it does**: Retrieves sales analytics for a specific dish
- **Input**: Path parameter:
  - `dish_id` (integer, required): Dish ID
- **Output**: Analytics object with:
  - `dish_id`: Dish ID
  - `dish_name`: Dish name
  - `total_sold`: Total quantity sold (requires Order module, currently returns 0)

## Database Schema
opening_hours | VARCHAR(100) | Operating hours |
| business_license_image | VARCHAR(255) | Business license image path |
| food_safety_certificate_image | VARCHAR(255) | Food safety cert image path |
| is_open | BOOLEAN | Current open/closed status |
| rating | NUMERIC(2,1) | Rating (0.0-5.0) |
| status | ENUM | PENDING/ACTIVE/BANNED/REJECTED |
| created_at | TIMESTAMP | Creation timestamp |

### Category Table

| Column | Type | Description |
|--------|------|-------------|
| id | INTEGER | Primary key |
| restaurant_id | INTEGER | Foreign key to restaurant |
| name | VARCHAR(100) | Category name |
| description | TEXT | Category description |
| dExample Requests

### Create a Restaurant with Images

*Windows PowerShell:*
```powershell
$fileLicense = Get-Item "C:\path\to\license.jpg"
$fileCert = Get-Item "C:\path\to\certificate.jpg"

curl.exe -X POST "http://localhost:8000/api/v1/restaurants/" `
  -F "owner_id=1" `
  -F "name=Pho 24" `
  -F "address=123 Main St, District 1, HCMC" `
  -F "phone=0901234567" `
  -F "description=Authentic Vietnamese noodle soup" `
  -F "opening_hours=08:00-22:00" `
  -F "business_license_image=@$($fileLicense.FullName)" `
  -F "food_safety_certificate_image=@$($fileCert.FullName)"
```

*Linux/Mac:*
```bash
curl -X POST "http://localhost:8000/api/v1/restaurants/" \
  -F "owner_id=1" \
  -F "name=Pho 24" \
  -F "address=123 Main St, District 1, HCMC" \
  -F "phone=0901234567" \
  -F "description=Authentic Vietnamese noodle soup" \
  -F "opening_hours=08:00-22:00" \
  -F "business_license_image=@/path/to/license.jpg" \
  -F "food_safety_certificate_image=@/path/to/certificate.jpg"
```

### Create a Category
```bash
curl -X POST "http://localhost:8000/api/v1/menu/restaurants/1/categories" \
  -H "Content-Type: application/json" \
  -d '{"name": "Main Courses", "description": "Traditional Vietnamese dishes"}'
```

### Create a Dish
```bash
curl -X POST "http://localhost:8000/api/v1/menu/restaurants/1/dishes" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Pho Bo",
    "description": "Beef noodle soup",
    "price": 50000,
    "category_id": 1,
    "is_available": true,
    "stock_quantity": 100
  }'
```

### Get All Restaurants
```bash
curl "http://localhost:8000/api/v1/restaurants/"
```

### Get Restaurant Menu
```bash
curl "http://localhost:8000/api/v1/menu/restaurants/1/dishes"
```

## TODO / Future Enhancements

- [ ] Add user authentication (JWT tokens)
- [ ] Add authorization checks (owner vs admin)
- [x] Create Users table and model
- [x] Add MenuItem management
- [x] Add restaurant image upload
- [ ] Add Order management (required for analytics)
- [ ] Implement search and filtering (by rating, cuisine)
- [ ] Add pagination helpers
- [ ] Add operating hours validation
- [ ] Add soft delete instead of hard delete
- [ ] Add audit logging
- [ ] Add unit tests
- [ ] Add integration tests
- [ ] Add image optimization and validation
- [ ] Add menu item image uploadStock availability |
| stock_quantity | INTEGER | Stock quantity |
| created_at | TIMESTAMP | Creation timestamp |
| updated_at | TIMESTAMP | Last updateto users table |
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
