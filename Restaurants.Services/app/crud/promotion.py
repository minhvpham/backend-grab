from sqlalchemy.orm import Session
from app.models.promotion import Promotion
from app.schemas.promotion import PromotionCreate, PromotionUpdate


def create_promotion(db: Session, promotion: PromotionCreate, restaurant_id: int):
    """Create a new promotion for a restaurant"""
    # Check if code exists for this restaurant
    existing = db.query(Promotion).filter(
        Promotion.restaurant_id == restaurant_id,
        Promotion.code == promotion.code
    ).first()
    if existing:
        return None  # Or raise specific error

    db_promo = Promotion(**promotion.model_dump(), restaurant_id=restaurant_id)
    db.add(db_promo)
    db.commit()
    db.refresh(db_promo)
    return db_promo


def get_restaurant_promotions(db: Session, restaurant_id: int):
    """Get all promotions for a specific restaurant"""
    return db.query(Promotion).filter(
        Promotion.restaurant_id == restaurant_id
    ).all()


def get_promotion_by_id(db: Session, promotion_id: int):
    """Get a single promotion by ID"""
    return db.query(Promotion).filter(Promotion.id == promotion_id).first()


def update_promotion(db: Session, promo_id: int, update_data: PromotionUpdate):
    """Update an existing promotion"""
    promo = db.query(Promotion).filter(Promotion.id == promo_id).first()
    if not promo:
        return None
        
    for key, value in update_data.model_dump(exclude_unset=True).items():
        setattr(promo, key, value)
        
    db.commit()
    db.refresh(promo)
    return promo


def delete_promotion(db: Session, promo_id: int):
    """Delete a promotion"""
    promo = db.query(Promotion).filter(Promotion.id == promo_id).first()
    if promo:
        db.delete(promo)
        db.commit()
        return True
    return False
