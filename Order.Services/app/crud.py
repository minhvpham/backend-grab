from sqlalchemy.orm import Session
from typing import Optional, List
from decimal import Decimal
import uuid

from . import models, schemas


# ========== Order CRUD ==========

def get_order(db: Session, order_id: str) -> Optional[models.Order]:
    return db.query(models.Order).filter(models.Order.id == uuid.UUID(order_id)).first()


def get_orders(db: Session, skip: int = 0, limit: int = 100) -> List[models.Order]:
    return db.query(models.Order).order_by(models.Order.created_at.desc()).offset(skip).limit(limit).all()


def get_orders_count(db: Session) -> int:
    return db.query(models.Order).count()


def get_orders_by_user(db: Session, user_id: str, skip: int = 0, limit: int = 100) -> List[models.Order]:
    return db.query(models.Order).filter(
        models.Order.user_id == user_id
    ).order_by(models.Order.created_at.desc()).offset(skip).limit(limit).all()


def get_orders_count_by_user(db: Session, user_id: str) -> int:
    return db.query(models.Order).filter(models.Order.user_id == user_id).count()


def get_orders_by_driver(db: Session, driver_id: str, skip: int = 0, limit: int = 100) -> List[models.Order]:
    return db.query(models.Order).filter(
        models.Order.driver_id == uuid.UUID(driver_id)
    ).order_by(models.Order.created_at.desc()).offset(skip).limit(limit).all()


def get_orders_by_restaurant(db: Session, restaurant_id: str, skip: int = 0, limit: int = 100) -> List[models.Order]:
    return db.query(models.Order).filter(
        models.Order.restaurant_id == uuid.UUID(restaurant_id)
    ).order_by(models.Order.created_at.desc()).offset(skip).limit(limit).all()


def create_order(db: Session, order: schemas.OrderCreate) -> models.Order:
    # Calculate totals
    subtotal = sum(item.quantity * item.unit_price for item in order.items)
    delivery_fee = Decimal("15000")  # Default delivery fee
    total_amount = subtotal + delivery_fee
    
    # Create order
    db_order = models.Order(
        user_id=order.user_id,
        restaurant_id=uuid.UUID(order.restaurant_id),
        delivery_address=order.delivery_address,
        delivery_note=order.delivery_note,
        payment_method=order.payment_method,
        subtotal=subtotal,
        delivery_fee=delivery_fee,
        total_amount=total_amount
    )
    db.add(db_order)
    db.flush()  # Get order ID
    
    # Create order items
    for item in order.items:
        db_item = models.OrderItem(
            order_id=db_order.id,
            product_id=uuid.UUID(item.product_id),
            product_name=item.product_name,
            quantity=item.quantity,
            unit_price=item.unit_price,
            note=item.note
        )
        db.add(db_item)
    
    db.commit()
    db.refresh(db_order)
    return db_order


def update_order(db: Session, order_id: str, order_update: schemas.OrderUpdate) -> Optional[models.Order]:
    db_order = get_order(db, order_id)
    if not db_order:
        return None
    
    update_data = order_update.model_dump(exclude_unset=True)
    
    if "status" in update_data and update_data["status"]:
        update_data["status"] = update_data["status"].value
    if "payment_status" in update_data and update_data["payment_status"]:
        update_data["payment_status"] = update_data["payment_status"].value
    if "driver_id" in update_data and update_data["driver_id"]:
        update_data["driver_id"] = uuid.UUID(update_data["driver_id"])
    
    for field, value in update_data.items():
        setattr(db_order, field, value)
    
    db.commit()
    db.refresh(db_order)
    return db_order


def delete_order(db: Session, order_id: str) -> bool:
    db_order = get_order(db, order_id)
    if not db_order:
        return False
    db.delete(db_order)
    db.commit()
    return True


def cancel_order(db: Session, order_id: str) -> Optional[models.Order]:
    db_order = get_order(db, order_id)
    if not db_order:
        return None
    if db_order.status in ["delivered", "cancelled"]:
        return None  # Cannot cancel delivered or already cancelled orders
    
    db_order.status = "cancelled"
    db.commit()
    db.refresh(db_order)
    return db_order


def assign_driver(db: Session, order_id: str, driver_id: str) -> Optional[models.Order]:
    """Assign a driver to an order"""
    db_order = get_order(db, order_id)
    if not db_order:
        return None
    
    db_order.driver_id = uuid.UUID(driver_id)
    db_order.status = "finding_driver"
    db.commit()
    db.refresh(db_order)
    return db_order
