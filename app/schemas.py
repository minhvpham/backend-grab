from pydantic import BaseModel, EmailStr, Field
from typing import Optional, List
from datetime import datetime
from enum import Enum
from decimal import Decimal


# ========== User Enums & Schemas ==========

class UserRole(str, Enum):
    USER = "user"
    SELLER = "seller"
    SHIPPER = "shipper"
    ADMIN = "admin"


class UserCreate(BaseModel):
    name: str = Field(..., min_length=1, max_length=255)
    email: EmailStr
    phone: str = Field(..., min_length=10, max_length=20)
    password: str = Field(..., min_length=8)
    role: UserRole = Field(default=UserRole.USER)
    avatar: Optional[str] = None
    address: Optional[str] = None


class UserUpdate(BaseModel):
    name: Optional[str] = Field(None, min_length=1, max_length=255)
    email: Optional[EmailStr] = None
    phone: Optional[str] = Field(None, min_length=10, max_length=20)
    password: Optional[str] = Field(None, min_length=8)
    role: Optional[UserRole] = None
    avatar: Optional[str] = None
    address: Optional[str] = None


class UserResponse(BaseModel):
    id: str
    name: str
    email: str
    phone: str
    role: UserRole
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
    id: str
    product_id: str
    product_name: str
    quantity: int
    unit_price: Decimal
    note: Optional[str]
    created_at: datetime

    class Config:
        from_attributes = True


# Order Schemas
class OrderCreate(BaseModel):
    user_id: str
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
    id: str
    user_id: str
    restaurant_id: str
    driver_id: Optional[str]
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


# ========== Response Wrappers ==========

class UserListResponse(BaseModel):
    success: bool = True
    message: str = "Success"
    data: List[UserResponse]
    total: int


class UserSingleResponse(BaseModel):
    success: bool = True
    message: str = "Success"
    data: UserResponse


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
