from fastapi import APIRouter, Depends, HTTPException, status, Query
from sqlalchemy.orm import Session
from typing import List, Optional
from app.db.base import get_db
from app.schemas.restaurant import RestaurantCreate, RestaurantUpdate, RestaurantResponse
from app.crud import restaurant as crud
from app.models.restaurant import RestaurantStatus

router = APIRouter()


@router.post("/", response_model=RestaurantResponse, status_code=status.HTTP_201_CREATED)
def create_restaurant(
    restaurant: RestaurantCreate, 
    db: Session = Depends(get_db),
    # TODO: Add authentication
    # current_user: User = Depends(get_current_user)
):
    """
    Create a new restaurant.
    
    **Note:** In production, owner_id should come from the authenticated user.
    For now, it's hardcoded to 1 for testing purposes.
    """
    # TODO: Replace hardcoded owner_id with current_user.id
    owner_id = 1  # TEMPORARY - Replace with authenticated user's ID
    return crud.create_restaurant(db, restaurant, owner_id=owner_id)


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
