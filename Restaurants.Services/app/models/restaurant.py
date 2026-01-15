import enum
from datetime import datetime
from sqlalchemy import Column, Integer, String, Text, Boolean, Numeric, ForeignKey, DateTime, Enum
from sqlalchemy.orm import relationship
from app.db.base import Base


class RestaurantStatus(str, enum.Enum):
    PENDING = "PENDING"
    ACTIVE = "ACTIVE"
    BANNED = "BANNED"
    REJECTED = "REJECTED"


class Restaurant(Base):
    __tablename__ = "restaurants"

    id = Column(Integer, primary_key=True, index=True)
    # Owner ID from Users table 
    owner_id = Column(Integer, ForeignKey("users.id"), nullable=False)
    
    name = Column(String(100), nullable=False)
    description = Column(Text, nullable=True)
    address = Column(Text, nullable=False)
    phone = Column(String(20), nullable=True)
    
    opening_hours = Column(String(100), nullable=True)
    is_open = Column(Boolean, default=True)
    rating = Column(Numeric(2, 1), default=0.0)
    
    # Document images for approval
    business_license_image = Column(String(255), nullable=True)
    food_safety_certificate_image = Column(String(255), nullable=True)
    
    # Status for admin approval 
    status = Column(Enum(RestaurantStatus), default=RestaurantStatus.PENDING)
    created_at = Column(DateTime, default=datetime.utcnow)
    
    # Relationships (can be added later when User, MenuItem models exist)
    # owner = relationship("User", back_populates="restaurants")
    # menu_items = relationship("MenuItem", back_populates="restaurant")
