from pydantic import BaseModel
from typing import Optional
from decimal import Decimal
from datetime import datetime
from app.models.restaurant import RestaurantStatus


class RestaurantBase(BaseModel):
    name: str
    description: Optional[str] = None
    address: str
    phone: Optional[str] = None
    opening_hours: Optional[str] = None
    business_license_image: Optional[str] = None
    food_safety_certificate_image: Optional[str] = None


class RestaurantCreate(RestaurantBase):
    pass  # Owner ID usually comes from the logged-in user token, not the body


class RestaurantUpdate(BaseModel):
    name: Optional[str] = None
    address: Optional[str] = None
    phone: Optional[str] = None
    description: Optional[str] = None
    is_open: Optional[bool] = None
    opening_hours: Optional[str] = None
    business_license_image: Optional[str] = None
    food_safety_certificate_image: Optional[str] = None
    # Only Admin should update status


class RestaurantResponse(RestaurantBase):
    id: int
    owner_id: int
    rating: float
    is_open: bool
    status: RestaurantStatus
    created_at: datetime

    class Config:
        from_attributes = True  # Allows Pydantic to read SQLAlchemy models
