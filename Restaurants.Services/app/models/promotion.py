import enum
from datetime import date
from sqlalchemy import Column, Integer, String, Float, ForeignKey, Date, Boolean, Enum, UniqueConstraint
from sqlalchemy.orm import relationship
from app.db.base import Base


class DiscountType(str, enum.Enum):
    PERCENTAGE = "percentage"  # e.g., 10%
    FIXED_AMOUNT = "fixed_amount"  # e.g., -20k VND


class Promotion(Base):
    __tablename__ = "promotions"

    id = Column(Integer, primary_key=True, index=True)
    restaurant_id = Column(Integer, ForeignKey("restaurants.id"), nullable=False)
    
    # The name of the promotion, e.g., "Summer Sale"
    name = Column(String(100), nullable=False)
    
    # The code users type, e.g., "SUMMER2025"
    code = Column(String(50), nullable=False, index=True)
    
    # Description for UI, e.g., "Get 20% off for all orders > 100k"
    description = Column(String(255), nullable=True)

    discount_type = Column(Enum(DiscountType), nullable=False)
    discount_value = Column(Float, nullable=False)  # Stores 20 (for %) or 20000 (for fixed)

    # Constraints
    min_order_value = Column(Float, default=0)  # Order must be > this
    max_discount_value = Column(Float, nullable=True)  # Cap for % discounts
    
    # Validity
    start_date = Column(Date, nullable=False)
    end_date = Column(Date, nullable=False)
    is_active = Column(Boolean, default=True)
    
    # Usage Limits (Optional but recommended)
    usage_limit = Column(Integer, nullable=True)  # Max global uses (e.g. 100 total)
    used_count = Column(Integer, default=0)

    # Relationships
    # restaurant = relationship("Restaurant", back_populates="promotions")

    # Constraint: A restaurant cannot have duplicate codes (but different restaurants can both use "WELCOME")
    __table_args__ = (
        UniqueConstraint('restaurant_id', 'code', name='uix_restaurant_code'),
    )
