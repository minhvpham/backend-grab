from sqlalchemy.orm import Session
from sqlalchemy import func
from typing import List, Optional
from app.models.menu import Category, MenuItem
from app.schemas.menu import (
    CategoryCreate, CategoryUpdate,
    MenuItemCreate, MenuItemUpdate
)
# You will need to import OrderItem here once created
# from app.models.order import OrderItem 


# ==================== CATEGORY CRUD ====================

def create_category(db: Session, category: CategoryCreate) -> Category:
    """Create a new global category"""
    db_obj = Category(**category.model_dump())
    db.add(db_obj)
    db.commit()
    db.refresh(db_obj)
    return db_obj


def get_category(db: Session, category_id: int) -> Optional[Category]:
    """Get a category by ID"""
    return db.query(Category).filter(Category.id == category_id).first()


def get_all_categories(db: Session) -> List[Category]:
    """Get all global categories"""
    return db.query(Category).all()


def update_category(db: Session, category_id: int, category_update: CategoryUpdate) -> Optional[Category]:
    """Update a category"""
    db_category = get_category(db, category_id)
    if not db_category:
        return None
    
    update_data = category_update.model_dump(exclude_unset=True)
    for key, value in update_data.items():
        setattr(db_category, key, value)
    
    db.add(db_category)
    db.commit()
    db.refresh(db_category)
    return db_category


def delete_category(db: Session, category_id: int) -> Optional[Category]:
    """Delete a category"""
    db_category = get_category(db, category_id)
    if db_category:
        db.delete(db_category)
        db.commit()
    return db_category


# ==================== MENU ITEM (DISH) CRUD ====================

def create_dish(db: Session, dish: MenuItemCreate, restaurant_id: int) -> MenuItem:
    """Create a new menu item (dish)"""
    db_dish = MenuItem(**dish.model_dump(), restaurant_id=restaurant_id)
    db.add(db_dish)
    db.commit()
    db.refresh(db_dish)
    return db_dish


def get_dish(db: Session, dish_id: int) -> Optional[MenuItem]:
    """Get a menu item by ID"""
    return db.query(MenuItem).filter(MenuItem.id == dish_id).first()


def get_dishes_by_restaurant(
    db: Session, 
    restaurant_id: int,
    category_id: Optional[int] = None,
    available_only: bool = False
) -> List[MenuItem]:
    """
    Get all menu items for a restaurant with optional filters
    
    Args:
        db: Database session
        restaurant_id: Restaurant ID
        category_id: Optional filter by category
        available_only: If True, only return available items
    """
    query = db.query(MenuItem).filter(MenuItem.restaurant_id == restaurant_id)
    
    if category_id is not None:
        query = query.filter(MenuItem.category_id == category_id)
    
    if available_only:
        query = query.filter(MenuItem.is_available == True)
    
    return query.all()


def get_dishes_by_category(db: Session, category_id: int) -> List[MenuItem]:
    """Get all menu items in a category"""
    return db.query(MenuItem).filter(MenuItem.category_id == category_id).all()


def update_dish(db: Session, dish_id: int, dish_update: MenuItemUpdate) -> Optional[MenuItem]:
    """Update a menu item"""
    db_dish = get_dish(db, dish_id)
    if not db_dish:
        return None
    
    update_data = dish_update.model_dump(exclude_unset=True)
    for key, value in update_data.items():
        setattr(db_dish, key, value)
    
    db.add(db_dish)
    db.commit()
    db.refresh(db_dish)
    return db_dish


def toggle_dish_availability(db: Session, dish_id: int) -> Optional[MenuItem]:
    """Toggle the is_available status of a dish"""
    db_dish = get_dish(db, dish_id)
    if not db_dish:
        return None
    
    db_dish.is_available = not db_dish.is_available
    db.add(db_dish)
    db.commit()
    db.refresh(db_dish)
    return db_dish


def delete_dish(db: Session, dish_id: int) -> Optional[MenuItem]:
    """Delete a menu item"""
    db_dish = get_dish(db, dish_id)
    if db_dish:
        db.delete(db_dish)
        db.commit()
    return db_dish


# ==================== ANALYTICS ====================

def get_dish_sold_quantity(db: Session, dish_id: int) -> int:
    """
    ANALYTICS FEATURE: Calculate total sold quantity for a dish
    
    This query sums the 'quantity' column from OrderItems for this dish
    Note: Requires OrderItem model to be defined later
    
    Args:
        db: Database session
        dish_id: Menu item ID
        
    Returns:
        Total quantity sold
    """
    # TODO: Uncomment once OrderItem model is created
    # from app.models.order import OrderItem
    # result = db.query(func.sum(OrderItem.quantity)).filter(
    #     OrderItem.menu_item_id == dish_id
    # ).scalar()
    # return result or 0
    
    return 0  # Placeholder until Order module is built
