from fastapi import APIRouter, Depends, HTTPException, status, Query, UploadFile, File, Form
from fastapi.responses import FileResponse
from sqlalchemy.orm import Session
from typing import List, Optional
from decimal import Decimal
from pathlib import Path
from app.db.base import get_db
from app.schemas.menu import (
    CategoryCreate, CategoryUpdate, CategoryResponse,
    MenuItemCreate, MenuItemUpdate, MenuItemResponse
)
from app.crud import menu as crud_menu
from app.utils import save_dish_image

router = APIRouter()


# ==================== CATEGORY ENDPOINTS ====================

@router.post("/categories", response_model=CategoryResponse, status_code=status.HTTP_201_CREATED)
def create_category(
    category: CategoryCreate, 
    db: Session = Depends(get_db)
):
    """
    Create a new global category.
    
    Categories are shared across all restaurants (e.g., "Đại hạ giá", "Ăn vặt", "Ăn trưa", "Đồ uống").
    """
    return crud_menu.create_category(db, category)


@router.get("/categories", response_model=List[CategoryResponse])
def list_all_categories(
    db: Session = Depends(get_db)
):
    """
    Get all global categories available for all restaurants.
    """
    return crud_menu.get_all_categories(db)


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
    Update a global category.
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
    Delete a global category.
    
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
async def create_dish(
    restaurant_id: int,
    name: str = Form(...),
    price: Decimal = Form(...),
    discounted_price: Optional[Decimal] = Form(None),
    description: Optional[str] = Form(None),
    category_id: Optional[int] = Form(None),
    is_available: bool = Form(True),
    stock_quantity: Optional[int] = Form(None),
    image: UploadFile = File(...),
    db: Session = Depends(get_db)
):
    """
    Create a new menu item (dish) for a restaurant with image upload.
    
    **Request (multipart/form-data):**
    - **name** (required): Dish name
    - **price** (required): Original price
    - **discounted_price** (optional): Price after discount
    - **description** (optional): Dish description
    - **category_id** (optional): Category assignment
    - **image** (optional): Dish image file (jpg, jpeg, png, gif, bmp, webp)
    - **is_available** (optional): Stock status (default: true)
    - **stock_quantity** (optional): Stock quantity
    
    **Returns:**
    - **id**: Dish ID
    - **restaurant_id**: Restaurant ID
    - **name**: Dish name
    - **price**: Original price
    - **discounted_price**: Discounted price (if set)
    - **description**: Description
    - **category_id**: Category ID
    - **image_url**: Image path (accessible via /uploads/dish_images/xxx.jpg)
    - **is_available**: Availability status
    - **stock_quantity**: Stock quantity
    """
    try:
        # Save uploaded image if provided
        image_url = None
        if image and image.filename:
            image_url = await save_dish_image(image)
        
        # Create dish data
        dish_data = MenuItemCreate(
            name=name,
            description=description,
            price=price,
            discounted_price=discounted_price,
            category_id=category_id,
            is_available=is_available,
            stock_quantity=stock_quantity
        )
        
        # Create dish in database
        db_dish = crud_menu.create_dish(db, dish_data, restaurant_id)
        
        # Update image_url if image was uploaded
        if image_url:
            db_dish.image_url = image_url
            db.commit()
            db.refresh(db_dish)
        
        return db_dish
        
    except ValueError as e:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail=str(e)
        )
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error creating dish: {str(e)}"
        )


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


@router.get("/dishes", response_model=List[MenuItemResponse])
def list_all_dishes(
    category_id: Optional[int] = Query(None, description="Filter by category"),
    available_only: bool = Query(False, description="Show only available items"),
    skip: int = Query(0, ge=0, description="Number of records to skip"),
    limit: int = Query(100, ge=1, le=500, description="Maximum number of records to return"),
    db: Session = Depends(get_db)
):
    """
    Get all menu items from all restaurants with optional filters.
    
    **Query Parameters:**
    - **category_id** (optional): Filter by specific category
    - **available_only** (optional): Only show items that are in stock (default: false)
    - **skip** (optional): Number of records to skip for pagination (default: 0)
    - **limit** (optional): Maximum number of records to return (default: 100, max: 500)
    
    **Returns:**
    List of dishes from all restaurants
    """
    return crud_menu.get_all_dishes(
        db,
        category_id=category_id,
        available_only=available_only,
        skip=skip,
        limit=limit
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
async def update_dish(
    dish_id: int,
    name: Optional[str] = Form(None),
    price: Optional[Decimal] = Form(None),
    discounted_price: Optional[Decimal] = Form(None),
    description: Optional[str] = Form(None),
    category_id: Optional[int] = Form(None),
    is_available: Optional[bool] = Form(None),
    stock_quantity: Optional[int] = Form(None),
    image: UploadFile = File(None),
    db: Session = Depends(get_db)
):
    """
    Update a menu item with all information including image.
    
    **Request (multipart/form-data):**
    - **name** (optional): Dish name
    - **price** (optional): Original price
    - **discounted_price** (optional): Price after discount
    - **description** (optional): Dish description
    - **category_id** (optional): Category assignment
    - **image** (optional): New dish image file (jpg, jpeg, png, gif, bmp, webp)
    - **is_available** (optional): Stock status
    - **stock_quantity** (optional): Stock quantity
    
    **Returns:**
    Updated dish information with all fields
    """
    try:
        # Check if dish exists
        db_dish = crud_menu.get_dish(db, dish_id)
        if not db_dish:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=f"Dish with id {dish_id} not found"
            )
        
        # Save new image if provided
        if image and image.filename:
            # Delete old image if exists
            if db_dish.image_url:
                old_image_path = Path(db_dish.image_url)
                if old_image_path.exists():
                    old_image_path.unlink()
            
            # Save new image
            new_image_url = await save_dish_image(image)
            db_dish.image_url = new_image_url
        
        # Update other fields if provided
        if name is not None:
            db_dish.name = name
        if price is not None:
            db_dish.price = price
        if discounted_price is not None:
            db_dish.discounted_price = discounted_price
        if description is not None:
            db_dish.description = description
        if category_id is not None:
            db_dish.category_id = category_id
        if is_available is not None:
            db_dish.is_available = is_available
        if stock_quantity is not None:
            db_dish.stock_quantity = stock_quantity
        
        db.commit()
        db.refresh(db_dish)
        return db_dish
        
    except ValueError as e:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail=str(e)
        )
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error updating dish: {str(e)}"
        )


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


@router.get("/restaurants/{restaurant_id}/dishes/{dish_id}/image")
def get_dish_image(
    restaurant_id: int,
    dish_id: int,
    db: Session = Depends(get_db)
):
    """
    Get the image for a specific dish.
    
    Returns the actual image file.
    """
    db_dish = crud_menu.get_dish(db, dish_id)
    if not db_dish:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Dish with id {dish_id} not found"
        )
    
    # Verify dish belongs to the restaurant
    if db_dish.restaurant_id != restaurant_id:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Dish with id {dish_id} not found in restaurant {restaurant_id}"
        )
    
    if not db_dish.image_url:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="Image not found for this dish"
        )
    
    image_path = Path(db_dish.image_url)
    if not image_path.exists():
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="Image file not found on server"
        )
    
    return FileResponse(image_path)
