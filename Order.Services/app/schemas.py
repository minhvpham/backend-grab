from pydantic import BaseModel, EmailStr, Field, field_serializer
from typing import Optional, List, Any
from datetime import datetime
from enum import Enum
from decimal import Decimal
from uuid import UUID


# ========== Profile Enums & Schemas ==========

class ProfileRole(str, Enum):
    USER = "user"
    SELLER = "seller"
    SHIPPER = "shipper"
    ADMIN = "admin"


class ProfileCreate(BaseModel):
    user_id: str = Field(..., description="ID từ Auth Service")
    name: str = Field(..., min_length=1, max_length=255)
    email: EmailStr
    phone: str = Field(..., min_length=10, max_length=20)
    avatar: Optional[str] = None
    address: Optional[str] = None


class ProfileUpdate(BaseModel):
    name: Optional[str] = Field(None, min_length=1, max_length=255)
    email: Optional[EmailStr] = None
    phone: Optional[str] = Field(None, min_length=10, max_length=20)
    role: Optional[ProfileRole] = None
    avatar: Optional[str] = None
    address: Optional[str] = None


class ProfileResponse(BaseModel):
    user_id: str  # ID từ Auth Service (Primary Key)
    name: str
    email: str
    phone: str
    role: str
    avatar: Optional[str]
    address: Optional[str]
    created_at: datetime
    updated_at: datetime

    class Config:
        from_attributes = True


# ========== Order Enums & Schemas ==========

class OrderStatus(str, Enum):
    PENDING = "pending"
    CONFIRMED = "confirmed"
    PREPARING = "preparing"
    READY = "ready"
    FINDING_DRIVER = "finding_driver"
    DELIVERING = "delivering"
    DELIVERED = "delivered"
    CANCELLED = "cancelled"
    PENDING_RESTAURANT = "pending_restaurant"
    RESTAURANT_REJECTED = "restaurant_rejected"
    RESTAURANT_ACCEPTED = "restaurant_accepted"
    DRIVER_ACCEPTED = "driver_accepted"
    DRIVER_REJECTED = "driver_rejected"


class PaymentStatus(str, Enum):
    UNPAID = "unpaid"
    PAID = "paid"
    REFUNDED = "refunded"


# Order Item Schemas
class OrderItemCreate(BaseModel):
    product_id: str
    product_name: str = Field(..., max_length=255)
    quantity: int = Field(..., ge=1)
    unit_price: Decimal = Field(..., ge=0)
    note: Optional[str] = None


class OrderItemResponse(BaseModel):
    id: Any
    product_id: Any
    product_name: str
    quantity: int
    unit_price: Decimal
    note: Optional[str]
    created_at: datetime

    class Config:
        from_attributes = True
    
    @field_serializer('id', 'product_id')
    def serialize_uuid(self, v):
        return str(v) if v else None


# Order Schemas
class OrderCreate(BaseModel):
    user_id: str  # ID từ Auth Service
    restaurant_id: str
    delivery_address: str
    delivery_note: Optional[str] = None
    payment_method: Optional[str] = None
    items: List[OrderItemCreate] = Field(..., min_length=1)


class OrderUpdate(BaseModel):
    status: Optional[OrderStatus] = None
    payment_status: Optional[PaymentStatus] = None
    driver_id: Optional[str] = None
    delivery_address: Optional[str] = None
    delivery_note: Optional[str] = None


class OrderResponse(BaseModel):
    id: Any
    user_id: str  # ID từ Auth Service
    restaurant_id: Any
    driver_id: Optional[Any]
    status: str
    payment_status: str
    payment_method: Optional[str]
    delivery_address: str
    delivery_note: Optional[str]
    subtotal: Decimal
    delivery_fee: Decimal
    discount: Decimal
    total_amount: Decimal
    items: List[OrderItemResponse] = []
    created_at: datetime
    updated_at: datetime

    class Config:
        from_attributes = True
    
    @field_serializer('id', 'restaurant_id', 'driver_id')
    def serialize_uuid(self, v):
        return str(v) if v else None


# ========== Response Wrappers ==========

class ProfileListResponse(BaseModel):
    success: bool = True
    message: str = "Success"
    data: List[ProfileResponse]
    total: int


class ProfileSingleResponse(BaseModel):
    success: bool = True
    message: str = "Success"
    data: ProfileResponse


class OrderListResponse(BaseModel):
    success: bool = True
    message: str = "Success"
    data: List[OrderResponse]
    total: int


class OrderSingleResponse(BaseModel):
    success: bool = True
    message: str = "Success"
    data: OrderResponse


class MessageResponse(BaseModel):
    success: bool
    message: str
