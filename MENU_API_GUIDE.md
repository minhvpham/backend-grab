# Dish & Menu Management API Guide

## Overview
The Menu Management Service allows restaurants to organize their food offerings using Categories and Menu Items (Dishes).

## Database Structure

```
Restaurant (1) ─── (Many) Categories ─── (Many) Menu Items
```

## API Endpoints

### Categories

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/menu/restaurants/{restaurant_id}/categories` | Create category |
| GET | `/api/v1/menu/restaurants/{restaurant_id}/categories` | List all categories |
| GET | `/api/v1/menu/categories/{category_id}` | Get category details |
| PUT | `/api/v1/menu/categories/{category_id}` | Update category |
| DELETE | `/api/v1/menu/categories/{category_id}` | Delete category |

### Menu Items (Dishes)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/menu/restaurants/{restaurant_id}/dishes` | Create dish |
| GET | `/api/v1/menu/restaurants/{restaurant_id}/dishes` | List dishes (with filters) |
| GET | `/api/v1/menu/categories/{category_id}/dishes` | List dishes by category |
| GET | `/api/v1/menu/dishes/{dish_id}` | Get dish details |
| PUT | `/api/v1/menu/dishes/{dish_id}` | Update dish |
| PATCH | `/api/v1/menu/dishes/{dish_id}/toggle-availability` | Toggle in stock/out of stock |
| DELETE | `/api/v1/menu/dishes/{dish_id}` | Delete dish |
| GET | `/api/v1/menu/dishes/{dish_id}/stats` | Get sales analytics |

## Example Usage

### Step 1: Create Categories

**Create "Main Courses" category:**
```powershell
Invoke-RestMethod -Uri "http://localhost:8000/api/v1/menu/restaurants/5/categories" -Method Post -ContentType "application/json" -Body (@{
    name = "Main Courses"
} | ConvertTo-Json)
```

**Windows CMD:**
```cmd
curl -X POST "http://localhost:8000/api/v1/menu/restaurants/5/categories" -H "Content-Type: application/json" -d "{\"name\":\"Main Courses\"}"
```

Response:
```json
{
  "id": 1,
  "restaurant_id": 5,
  "name": "Main Courses"
}
```

### Step 2: Create Menu Items

**Add a dish to the category:**

*PowerShell:*
```powershell
Invoke-RestMethod -Uri "http://localhost:8000/api/v1/menu/restaurants/5/dishes" -Method Post -ContentType "application/json" -Body (@{
    name = "Pho Bo"
    description = "Traditional Vietnamese beef noodle soup"
    price = 65000
    category_id = 1
    image_url = "https://example.com/pho-bo.jpg"
    is_available = $true
    stock_quantity = 50
} | ConvertTo-Json)
```

*Windows CMD:*
```cmd
curl -X POST "http://localhost:8000/api/v1/menu/restaurants/5/dishes" -H "Content-Type: application/json" -d "{\"name\":\"Pho Bo\",\"description\":\"Traditional Vietnamese beef noodle soup\",\"price\":65000,\"category_id\":1,\"is_available\":true}"
```

Response:
```json
{
  "id": 1,
  "restaurant_id": 5,
  "category_id": 1,
  "name": "Pho Bo",
  "description": "Traditional Vietnamese beef noodle soup",
  "price": "65000.00",
  "image_url": "https://example.com/pho-bo.jpg",
  "is_available": true,
  "stock_quantity": 50
}
```

### Step 3: List All Dishes

**Get all dishes for restaurant:**
```cmd
curl "http://localhost:8000/api/v1/menu/restaurants/5/dishes"
```

**Get only available dishes:**
```cmd
curl "http://localhost:8000/api/v1/menu/restaurants/5/dishes?available_only=true"
```

**Filter by category:**
```cmd
curl "http://localhost:8000/api/v1/menu/restaurants/5/dishes?category_id=1"
```

### Step 4: Update Dish

**Change price and description:**

*PowerShell:*
```powershell
Invoke-RestMethod -Uri "http://localhost:8000/api/v1/menu/dishes/1" -Method Put -ContentType "application/json" -Body (@{
    price = 70000
    description = "Premium Vietnamese beef noodle soup with extra meat"
} | ConvertTo-Json)
```

*Windows CMD:*
```cmd
curl -X PUT "http://localhost:8000/api/v1/menu/dishes/1" -H "Content-Type: application/json" -d "{\"price\":70000,\"description\":\"Premium Vietnamese beef noodle soup with extra meat\"}"
```

### Step 5: Inventory Management

**Mark dish as out of stock (toggle):**
```cmd
curl -X PATCH "http://localhost:8000/api/v1/menu/dishes/1/toggle-availability"
```

**Update stock quantity:**
```cmd
curl -X PUT "http://localhost:8000/api/v1/menu/dishes/1" -H "Content-Type: application/json" -d "{\"stock_quantity\":0,\"is_available\":false}"
```

### Step 6: Get Analytics

**Get sales statistics for a dish:**
```cmd
curl "http://localhost:8000/api/v1/menu/dishes/1/stats"
```

Response:
```json
{
  "dish_id": 1,
  "dish_name": "Pho Bo",
  "total_sold": 0
}
```
*Note: Will return actual data once Order module is implemented*

## Complete Workflow Example

```cmd
# 1. Create categories
curl -X POST "http://localhost:8000/api/v1/menu/restaurants/5/categories" -H "Content-Type: application/json" -d "{\"name\":\"Appetizers\"}"
curl -X POST "http://localhost:8000/api/v1/menu/restaurants/5/categories" -H "Content-Type: application/json" -d "{\"name\":\"Main Courses\"}"
curl -X POST "http://localhost:8000/api/v1/menu/restaurants/5/categories" -H "Content-Type: application/json" -d "{\"name\":\"Drinks\"}"

# 2. Add dishes
curl -X POST "http://localhost:8000/api/v1/menu/restaurants/5/dishes" -H "Content-Type: application/json" -d "{\"name\":\"Spring Rolls\",\"price\":35000,\"category_id\":1}"
curl -X POST "http://localhost:8000/api/v1/menu/restaurants/5/dishes" -H "Content-Type: application/json" -d "{\"name\":\"Pho Bo\",\"price\":65000,\"category_id\":2}"
curl -X POST "http://localhost:8000/api/v1/menu/restaurants/5/dishes" -H "Content-Type: application/json" -d "{\"name\":\"Iced Coffee\",\"price\":25000,\"category_id\":3}"

# 3. List all dishes
curl "http://localhost:8000/api/v1/menu/restaurants/5/dishes"

# 4. Update a dish
curl -X PUT "http://localhost:8000/api/v1/menu/dishes/2" -H "Content-Type: application/json" -d "{\"price\":70000}"

# 5. Mark as out of stock
curl -X PATCH "http://localhost:8000/api/v1/menu/dishes/2/toggle-availability"
```

## Database Migration

After implementing the models, run:

```bash
alembic revision --autogenerate -m "create menu and categories tables"
alembic upgrade head
```

## Testing with Swagger UI

Visit http://localhost:8000/docs to test all endpoints interactively.

The menu endpoints are under the **"menu"** tag.

## Notes

- All menu items must belong to a restaurant
- Categories are optional but recommended for organization
- Use `is_available` for quick inventory control
- The analytics endpoint will provide real data once the Order module is implemented
- Consider using soft delete (is_available=false) instead of hard delete to preserve order history
