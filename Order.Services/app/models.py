from sqlalchemy import Column, String, DateTime, Text, ForeignKey, Integer, Numeric
from sqlalchemy.dialects.postgresql import UUID
from sqlalchemy.orm import relationship
from datetime import datetime
import uuid
import enum

from .database import Base


class ProfileRole(str, enum.Enum):
    """Profile roles in the system"""
    USER = "user"
    SELLER = "seller"
    SHIPPER = "shipper"
    ADMIN = "admin"


class OrderStatus(str, enum.Enum):
    """Order status enum"""
    PENDING = "pending"
    CONFIRMED = "confirmed"
    PREPARING = "preparing"
    READY = "ready"
    FINDING_DRIVER = "finding_driver"
    DELIVERING = "delivering"
    DELIVERED = "delivered"
    CANCELLED = "cancelled"


class PaymentStatus(str, enum.Enum):
    """Payment status enum"""
    UNPAID = "unpaid"
    PAID = "paid"
    REFUNDED = "refunded"


class Profile(Base):
    """Profile model for database - quản lý thông tin cá nhân người dùng"""
    __tablename__ = "profiles"

    id = Column(UUID(as_uuid=True), primary_key=True, default=uuid.uuid4)
    name = Column(String(255), nullable=False)
    email = Column(String(255), unique=True, nullable=False, index=True)
    phone = Column(String(20), unique=True, nullable=False, index=True)
    password = Column(String(255), nullable=False)
    role = Column(String(20), default="user", nullable=False)
    avatar = Column(String(500), nullable=True)
    address = Column(Text, nullable=True)
    
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    # Relationship to orders
    orders = relationship("Order", back_populates="profile")

    def __repr__(self):
        return f"<Profile(id={self.id}, name={self.name}, email={self.email})>"


class Order(Base):
    """Order model for database"""
    __tablename__ = "orders"

    id = Column(UUID(as_uuid=True), primary_key=True, default=uuid.uuid4)
    profile_id = Column(UUID(as_uuid=True), ForeignKey("profiles.id"), nullable=False, index=True)
    restaurant_id = Column(UUID(as_uuid=True), nullable=False, index=True)  # FK to Restaurant service
    driver_id = Column(UUID(as_uuid=True), nullable=True, index=True)  # FK to Driver service
    
    status = Column(String(30), default="pending", nullable=False)
    payment_status = Column(String(20), default="unpaid", nullable=False)
    payment_method = Column(String(50), nullable=True)
    
    delivery_address = Column(Text, nullable=False)
    delivery_note = Column(Text, nullable=True)
    
    subtotal = Column(Numeric(12, 2), default=0)
    delivery_fee = Column(Numeric(12, 2), default=0)
    discount = Column(Numeric(12, 2), default=0)
    total_amount = Column(Numeric(12, 2), default=0)
    
    created_at = Column(DateTime, default=datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.utcnow, onupdate=datetime.utcnow)

    # Relationships
    profile = relationship("Profile", back_populates="orders")
    items = relationship("OrderItem", back_populates="order", cascade="all, delete-orphan")

    def __repr__(self):
        return f"<Order(id={self.id}, profile_id={self.profile_id}, status={self.status})>"


class OrderItem(Base):
    """Order item model"""
    __tablename__ = "order_items"

    id = Column(UUID(as_uuid=True), primary_key=True, default=uuid.uuid4)
    order_id = Column(UUID(as_uuid=True), ForeignKey("orders.id"), nullable=False)
    product_id = Column(UUID(as_uuid=True), nullable=False)  # FK to Restaurant service menu
    
    product_name = Column(String(255), nullable=False)
    quantity = Column(Integer, nullable=False, default=1)
    unit_price = Column(Numeric(12, 2), nullable=False)
    note = Column(Text, nullable=True)
    
    created_at = Column(DateTime, default=datetime.utcnow)

    # Relationship
    order = relationship("Order", back_populates="items")

    def __repr__(self):
        return f"<OrderItem(id={self.id}, product_name={self.product_name}, qty={self.quantity})>"
