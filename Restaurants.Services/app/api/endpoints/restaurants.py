from fastapi import APIRouter, Depends, HTTPException, status, Query, UploadFile, File, Form
from sqlalchemy.orm import Session
from typing import List, Optional
from app.db.base import get_db
from app.schemas.restaurant import RestaurantCreate, RestaurantUpdate, RestaurantResponse
from app.crud import restaurant as crud
from app.models.restaurant import RestaurantStatus
from app.utils import save_business_license, save_food_safety_certificate, ensure_upload_directories

router = APIRouter()

# Ensure upload directories exist on startup
ensure_upload_directories()


@router.post("/", response_model=RestaurantResponse, status_code=status.HTTP_201_CREATED)
async def create_restaurant(
    owner_id: int = Form(...),
    name: str = Form(...),
    address: str = Form(...),
    phone: Optional[str] = Form(None),
    description: Optional[str] = Form(None),
    opening_hours: Optional[str] = Form(None),
    business_license_image: UploadFile = File(...),
    food_safety_certificate_image: UploadFile = File(...),
    db: Session = Depends(get_db),
):
    """
    Create a new restaurant with document images.
    
    This endpoint accepts multipart/form-data with:
    - owner_id: User ID of the restaurant owner
    - Restaurant details as form fields
    - Two image files (business license and food safety certificate)
    """
    try:
        # Save uploaded images
        business_license_path = await save_business_license(business_license_image)
        food_safety_cert_path = await save_food_safety_certificate(food_safety_certificate_image)
        
        # Create restaurant data
        restaurant_data = RestaurantCreate(
            name=name,
            description=description,
            address=address,
            phone=phone,
            opening_hours=opening_hours,
            business_license_image=business_license_path,
            food_safety_certificate_image=food_safety_cert_path
        )
        
        return crud.create_restaurant(db, restaurant_data, owner_id=owner_id)
        
    except ValueError as e:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail=str(e)
        )
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Error creating restaurant: {str(e)}"
        )


@router.get("/", response_model=List[RestaurantResponse])
def list_restaurants(
    skip: int = Query(0, ge=0, description="Number of records to skip"),
    limit: int = Query(100, ge=1, le=100, description="Maximum number of records to return"),
    status: Optional[RestaurantStatus] = Query(None, description="Filter by restaurant status"),
    db: Session = Depends(get_db)
):
    """
    Get a list of restaurants with optional filtering.
    
    - **skip**: Number of records to skip (for pagination)
    - **limit**: Maximum number of records to return (1-100)
    - **status**: Filter by restaurant status (PENDING, ACTIVE, BANNED, REJECTED)
    """
    restaurants = crud.get_restaurants(db, skip=skip, limit=limit, status=status)
    return restaurants


@router.get("/{restaurant_id}", response_model=RestaurantResponse)
def read_restaurant(
    restaurant_id: int, 
    db: Session = Depends(get_db)
):
    """
    Get a specific restaurant by ID.
    """
    db_restaurant = crud.get_restaurant(db, restaurant_id)
    if db_restaurant is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Restaurant with id {restaurant_id} not found"
        )
    return db_restaurant


@router.get("/owner/{owner_id}", response_model=List[RestaurantResponse])
def list_owner_restaurants(
    owner_id: int,
    db: Session = Depends(get_db)
):
    """
    Get all restaurants owned by a specific user.
    """
    restaurants = crud.get_restaurants_by_owner(db, owner_id)
    return restaurants


@router.put("/{restaurant_id}", response_model=RestaurantResponse)
def update_restaurant(
    restaurant_id: int, 
    restaurant: RestaurantUpdate, 
    db: Session = Depends(get_db),
    # TODO: Add authentication and authorization
    # current_user: User = Depends(get_current_user)
):
    """
    Update an existing restaurant.
    
    **Note:** In production, should verify that current_user is the owner.
    """
    # TODO: Verify ownership - current_user.id == restaurant.owner_id
    db_restaurant = crud.update_restaurant(db, restaurant_id, restaurant)
    if db_restaurant is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Restaurant with id {restaurant_id} not found"
        )
    return db_restaurant


@router.patch("/{restaurant_id}/toggle-open", response_model=RestaurantResponse)
def toggle_restaurant_open(
    restaurant_id: int,
    db: Session = Depends(get_db),
    # TODO: Add authentication and authorization
    # current_user: User = Depends(get_current_user)
):
    """
    Toggle the open/closed status of a restaurant.
    
    **Note:** In production, should verify that current_user is the owner.
    """
    db_restaurant = crud.toggle_restaurant_open_status(db, restaurant_id)
    if db_restaurant is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Restaurant with id {restaurant_id} not found"
        )
    return db_restaurant


@router.patch("/{restaurant_id}/status", response_model=RestaurantResponse)
def update_restaurant_status(
    restaurant_id: int,
    new_status: RestaurantStatus,
    db: Session = Depends(get_db),
    # TODO: Add authentication and admin authorization
    # current_user: User = Depends(get_current_admin_user)
):
    """
    Update restaurant status (Admin only).
    
    Used for the restaurant approval workflow:
    - PENDING -> ACTIVE (approve)
    - PENDING -> REJECTED (reject)
    - ACTIVE -> BANNED (ban)
    
    **Note:** This endpoint should only be accessible to admins.
    """
    db_restaurant = crud.update_restaurant_status(db, restaurant_id, new_status)
    if db_restaurant is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Restaurant with id {restaurant_id} not found"
        )
    return db_restaurant


@router.delete("/{restaurant_id}", status_code=status.HTTP_204_NO_CONTENT)
def delete_restaurant(
    restaurant_id: int,
    db: Session = Depends(get_db),
    # TODO: Add authentication and authorization
    # current_user: User = Depends(get_current_user)
):
    """
    Delete a restaurant (hard delete).
    
    **Note:** In production:
    - Should verify that current_user is the owner or an admin
    - Consider using soft delete (status = DELETED) instead
    """
    db_restaurant = crud.delete_restaurant(db, restaurant_id)
    if db_restaurant is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"Restaurant with id {restaurant_id} not found"
        )
    return None
