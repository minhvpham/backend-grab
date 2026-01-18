from fastapi import APIRouter, Depends, HTTPException
from typing import List
from sqlalchemy.orm import Session
from app.db.base import get_db
from app.schemas.promotion import PromotionCreate, PromotionResponse, PromotionUpdate
from app.crud import promotion as crud_promo

router = APIRouter()


@router.post("/restaurants/{restaurant_id}/promotions", response_model=PromotionResponse, status_code=201)
def create_discount(
    restaurant_id: int, 
    promotion: PromotionCreate, 
    db: Session = Depends(get_db)
):
    """
    Create a new promotion/discount for a restaurant.
    
    - **restaurant_id**: ID of the restaurant
    - **code**: Uppercase alphanumeric code (3-20 chars)
    - **discount_type**: Either "percentage" or "fixed_amount"
    - **discount_value**: The discount value (0-100 for percentage, amount for fixed)
    - **min_order_value**: Minimum order value required
    - **max_discount_value**: Maximum discount cap (useful for percentage discounts)
    - **start_date**: When the promotion starts
    - **end_date**: When the promotion ends
    - **usage_limit**: Maximum number of times this promotion can be used
    """
    result = crud_promo.create_promotion(db, promotion, restaurant_id)
    if not result:
        raise HTTPException(
            status_code=400, 
            detail="Promotion code already exists for this restaurant"
        )
    return result


@router.get("/restaurants/{restaurant_id}/promotions", response_model=List[PromotionResponse])
def list_discounts(restaurant_id: int, db: Session = Depends(get_db)):
    """
    Get all promotions for a specific restaurant.
    
    - **restaurant_id**: ID of the restaurant
    """
    return crud_promo.get_restaurant_promotions(db, restaurant_id)


@router.get("/promotions/{promotion_id}", response_model=PromotionResponse)
def get_discount(promotion_id: int, db: Session = Depends(get_db)):
    """
    Get a specific promotion by its ID.
    
    - **promotion_id**: ID of the promotion
    """
    promotion = crud_promo.get_promotion_by_id(db, promotion_id)
    if not promotion:
        raise HTTPException(status_code=404, detail="Promotion not found")
    return promotion


@router.put("/promotions/{promotion_id}", response_model=PromotionResponse)
def update_discount(
    promotion_id: int, 
    promo_in: PromotionUpdate, 
    db: Session = Depends(get_db)
):
    """
    Update an existing promotion.
    
    - **promotion_id**: ID of the promotion
    - Only fields provided in the request will be updated
    """
    result = crud_promo.update_promotion(db, promotion_id, promo_in)
    if not result:
        raise HTTPException(status_code=404, detail="Promotion not found")
    return result


@router.delete("/promotions/{promotion_id}")
def delete_discount(promotion_id: int, db: Session = Depends(get_db)):
    """
    Delete a promotion.
    
    - **promotion_id**: ID of the promotion to delete
    """
    success = crud_promo.delete_promotion(db, promotion_id)
    if not success:
        raise HTTPException(status_code=404, detail="Promotion not found")
    return {"message": "Promotion deleted successfully"}
