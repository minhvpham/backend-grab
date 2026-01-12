from pydantic import BaseModel, Field
from typing import Optional, List
from datetime import datetime
from enum import Enum
from decimal import Decimal


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
