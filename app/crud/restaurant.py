from sqlalchemy.orm import Session
from typing import List, Optional
from app.models.restaurant import Restaurant, RestaurantStatus
from app.schemas.restaurant import RestaurantCreate, RestaurantUpdate


def create_restaurant(db: Session, restaurant: RestaurantCreate, owner_id: int) -> Restaurant:
    """
    Create a new restaurant for a given owner.
    
    Args:
        db: Database session
        restaurant: Restaurant creation data
        owner_id: ID of the user who owns the restaurant
        
    Returns:
        Created Restaurant object
    """
    db_restaurant = Restaurant(**restaurant.model_dump(), owner_id=owner_id)
    db.add(db_restaurant)
    db.commit()
    db.refresh(db_restaurant)
    return db_restaurant


def get_restaurant(db: Session, restaurant_id: int) -> Optional[Restaurant]:
    """
    Get a single restaurant by ID.
    
    Args:
        db: Database session
        restaurant_id: ID of the restaurant
        
    Returns:
        Restaurant object or None if not found
    """
    return db.query(Restaurant).filter(Restaurant.id == restaurant_id).first()


def get_restaurants(
    db: Session, 
    skip: int = 0, 
    limit: int = 100,
    status: Optional[RestaurantStatus] = None
) -> List[Restaurant]:
    """
    Get a list of restaurants with optional filtering.
    
    Args:
        db: Database session
        skip: Number of records to skip (for pagination)
        limit: Maximum number of records to return
        status: Optional status filter
        
    Returns:
        List of Restaurant objects
    """
    query = db.query(Restaurant)
    
    if status:
        query = query.filter(Restaurant.status == status)
    
    return query.offset(skip).limit(limit).all()


def get_restaurants_by_owner(db: Session, owner_id: int) -> List[Restaurant]:
    """
    Get all restaurants owned by a specific user.
    
    Args:
        db: Database session
        owner_id: ID of the owner
        
    Returns:
        List of Restaurant objects
    """
    return db.query(Restaurant).filter(Restaurant.owner_id == owner_id).all()


def update_restaurant(
    db: Session, 
    restaurant_id: int, 
    restaurant_update: RestaurantUpdate
) -> Optional[Restaurant]:
    """
    Update an existing restaurant.
    
    Args:
        db: Database session
        restaurant_id: ID of the restaurant to update
        restaurant_update: Update data
        
    Returns:
        Updated Restaurant object or None if not found
    """
    db_restaurant = get_restaurant(db, restaurant_id)
    if not db_restaurant:
        return None
    
    update_data = restaurant_update.model_dump(exclude_unset=True)
    for key, value in update_data.items():
        setattr(db_restaurant, key, value)
    
    db.add(db_restaurant)
    db.commit()
    db.refresh(db_restaurant)
    return db_restaurant


def delete_restaurant(db: Session, restaurant_id: int) -> Optional[Restaurant]:
    """
    Delete a restaurant (hard delete).
    Usually we deactivate instead of delete, but this is for completeness.
    
    Args:
        db: Database session
        restaurant_id: ID of the restaurant to delete
        
    Returns:
        Deleted Restaurant object or None if not found
    """
    db_restaurant = get_restaurant(db, restaurant_id)
    if db_restaurant:
        db.delete(db_restaurant)
        db.commit()
    return db_restaurant


def update_restaurant_status(
    db: Session,
    restaurant_id: int,
    status: RestaurantStatus
) -> Optional[Restaurant]:
    """
    Update restaurant status (for admin approval workflow).
    
    Args:
        db: Database session
        restaurant_id: ID of the restaurant
        status: New status to set
        
    Returns:
        Updated Restaurant object or None if not found
    """
    db_restaurant = get_restaurant(db, restaurant_id)
    if not db_restaurant:
        return None
    
    db_restaurant.status = status
    db.add(db_restaurant)
    db.commit()
    db.refresh(db_restaurant)
    return db_restaurant


def toggle_restaurant_open_status(
    db: Session,
    restaurant_id: int
) -> Optional[Restaurant]:
    """
    Toggle the is_open status of a restaurant.
    
    Args:
        db: Database session
        restaurant_id: ID of the restaurant
        
    Returns:
        Updated Restaurant object or None if not found
    """
    db_restaurant = get_restaurant(db, restaurant_id)
    if not db_restaurant:
        return None
    
    db_restaurant.is_open = not db_restaurant.is_open
    db.add(db_restaurant)
    db.commit()
    db.refresh(db_restaurant)
    return db_restaurant
