from pydantic import BaseModel
from typing import Optional, List
from decimal import Decimal


# --- Category Schemas ---
class CategoryCreate(BaseModel):
    name: str


class CategoryUpdate(BaseModel):
    name: Optional[str] = None


class CategoryResponse(BaseModel):
    id: int
    restaurant_id: int
    name: str
    
    class Config:
        from_attributes = True


# --- MenuItem (Dish) Schemas ---
class MenuItemBase(BaseModel):
    name: str
    description: Optional[str] = None
    price: Decimal
    image_url: Optional[str] = None
    category_id: Optional[int] = None
    is_available: bool = True


class MenuItemCreate(MenuItemBase):
    pass


class MenuItemUpdate(BaseModel):
    name: Optional[str] = None
    description: Optional[str] = None
    price: Optional[Decimal] = None
    image_url: Optional[str] = None
    category_id: Optional[int] = None
    is_available: Optional[bool] = None  # For quick stock toggling
    stock_quantity: Optional[int] = None


class MenuItemResponse(MenuItemBase):
    id: int
    restaurant_id: int
    stock_quantity: Optional[int] = None
    
    class Config:
        from_attributes = True
