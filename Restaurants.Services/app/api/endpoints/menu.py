from fastapi import APIRouter, Depends, HTTPException, status, Query
from sqlalchemy.orm import Session
from typing import List, Optional
from app.db.base import get_db
from app.schemas.menu import (
    CategoryCreate, CategoryUpdate, CategoryResponse,
    MenuItemCreate, MenuItemUpdate, MenuItemResponse
)
from app.crud import menu as crud_menu

router = APIRouter()


# ==================== CATEGORY ENDPOINTS ====================

@router.post("/restaurants/{restaurant_id}/categories", response_model=CategoryResponse, status_code=status.HTTP_201_CREATED)
def create_category(
    restaurant_id: int, 
    category: CategoryCreate, 
    db: Session = Depends(get_db)
):
    """
    Create a new category for a restaurant.
    
    Categories help organize menu items (e.g., "Appetizers", "Main Courses", "Drinks").
    """
    return crud_menu.create_category(db, category, restaurant_id)


@router.get("/restaurants/{restaurant_id}/categories", response_model=List[CategoryResponse])
def list_categories(
    restaurant_id: int,
    db: Session = Depends(get_db)
):
    """
    Get all categories for a specific restaurant.
    """
    return crud_menu.get_categories_by_restaurant(db, restaurant_id)


@router.get("/categories/{category_id}", response_model=CategoryResponse)
def get_category(
    category_id: int,
    db: Session = Depends(get_db)
):
    """
    Get a specific category by ID.
    """
    db_category = crud_menu.get_category(db, category_id)
    if not db_category:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {category_id} not found"
        )
    return db_category


@router.put("/categories/{category_id}", response_model=CategoryResponse)
def update_category(
    category_id: int,
    category: CategoryUpdate,
    db: Session = Depends(get_db)
):
    """
    Update a category.
    """
    db_category = crud_menu.update_category(db, category_id, category)
    if not db_category:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {category_id} not found"
        )
    return db_category


@router.delete("/categories/{category_id}", status_code=status.HTTP_204_NO_CONTENT)
def delete_category(
    category_id: int,
    db: Session = Depends(get_db)
):
    """
    Delete a category.
    
    **Warning:** This will also affect menu items linked to this category.
    """
    db_category = crud_menu.delete_category(db, category_id)
    if not db_category:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Category with id {category_id} not found"
        )
    return None


# ==================== MENU ITEM (DISH) ENDPOINTS ====================

@router.post("/restaurants/{restaurant_id}/dishes", response_model=MenuItemResponse, status_code=status.HTTP_201_CREATED)
def create_dish(
    restaurant_id: int, 
    dish: MenuItemCreate, 
    db: Session = Depends(get_db)
):
    """
    Create a new menu item (dish) for a restaurant.
    
    - **name**: Dish name
    - **description**: Optional description
    - **price**: Price in your currency
    - **category_id**: Optional category assignment
    - **image_url**: Optional image URL
    - **is_available**: Stock status (default: true)
    """
    return crud_menu.create_dish(db, dish, restaurant_id)


@router.get("/restaurants/{restaurant_id}/dishes", response_model=List[MenuItemResponse])
def list_dishes(
    restaurant_id: int,
    category_id: Optional[int] = Query(None, description="Filter by category"),
    available_only: bool = Query(False, description="Show only available items"),
    db: Session = Depends(get_db)
):
    """
    Get all menu items for a restaurant with optional filters.
    
    - **category_id**: Filter by specific category
    - **available_only**: Only show items that are in stock
    """
    return crud_menu.get_dishes_by_restaurant(
        db, 
        restaurant_id, 
        category_id=category_id,
        available_only=available_only
    )


@router.get("/categories/{category_id}/dishes", response_model=List[MenuItemResponse])
def list_dishes_by_category(
    category_id: int,
    db: Session = Depends(get_db)
):
    """
    Get all menu items in a specific category.
    """
    return crud_menu.get_dishes_by_category(db, category_id)


@router.get("/dishes/{dish_id}", response_model=MenuItemResponse)
def get_dish(
    dish_id: int,
    db: Session = Depends(get_db)
):
    """
    Get a specific menu item by ID.
    """
    db_dish = crud_menu.get_dish(db, dish_id)
    if not db_dish:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Dish with id {dish_id} not found"
        )
    return db_dish


@router.put("/dishes/{dish_id}", response_model=MenuItemResponse)
def update_dish(
    dish_id: int, 
    dish: MenuItemUpdate, 
    db: Session = Depends(get_db)
):
    """
    Update a menu item.
    
    Can be used to:
    - Edit dish information (name, price, description)
    - Toggle availability (is_available)
    - Update stock quantity
    """
    db_dish = crud_menu.update_dish(db, dish_id, dish)
    if not db_dish:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Dish with id {dish_id} not found"
        )
    return db_dish


@router.patch("/dishes/{dish_id}/toggle-availability", response_model=MenuItemResponse)
def toggle_dish_availability(
    dish_id: int,
    db: Session = Depends(get_db)
):
    """
    Quick toggle for dish availability (in stock / out of stock).
    
    Useful for inventory management - quickly mark items as unavailable
    without changing other details.
    """
    db_dish = crud_menu.toggle_dish_availability(db, dish_id)
    if not db_dish:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Dish with id {dish_id} not found"
        )
    return db_dish


@router.delete("/dishes/{dish_id}", status_code=status.HTTP_204_NO_CONTENT)
def delete_dish(
    dish_id: int,
    db: Session = Depends(get_db)
):
    """
    Delete a menu item.
    
    **Note:** Consider using the availability toggle instead of deleting
    to preserve order history.
    """
    db_dish = crud_menu.delete_dish(db, dish_id)
    if not db_dish:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Dish with id {dish_id} not found"
        )
    return None


# ==================== ANALYTICS ENDPOINTS ====================

@router.get("/dishes/{dish_id}/stats")
def get_dish_stats(
    dish_id: int, 
    db: Session = Depends(get_db)
):
    """
    Get analytics for a specific dish.
    
    Returns:
    - **total_sold**: Total quantity sold (requires Order module)
    
    **Note:** Currently returns 0 until Order module is implemented.
    """
    # Verify dish exists
    db_dish = crud_menu.get_dish(db, dish_id)
    if not db_dish:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Dish with id {dish_id} not found"
        )
    
    quantity = crud_menu.get_dish_sold_quantity(db, dish_id)
    return {
        "dish_id": dish_id,
        "dish_name": db_dish.name,
        "total_sold": quantity
    }
