from datetime import datetime
from sqlalchemy import Column, Integer, String, Text, Boolean, Numeric, ForeignKey, DateTime
from sqlalchemy.orm import relationship
from app.db.base import Base


class Category(Base):
    __tablename__ = "categories"

    id = Column(Integer, primary_key=True, index=True)
    # Link to Restaurant
    restaurant_id = Column(Integer, ForeignKey("restaurants.id"), nullable=False)
    name = Column(String(100), nullable=False)

    # Relationships
    items = relationship("MenuItem", back_populates="category")
    # restaurant = relationship("Restaurant", back_populates="categories")


class MenuItem(Base):
    __tablename__ = "menu_items"

    id = Column(Integer, primary_key=True, index=True)
    restaurant_id = Column(Integer, ForeignKey("restaurants.id"), nullable=False)
    category_id = Column(Integer, ForeignKey("categories.id"), nullable=True)

    name = Column(String(100), nullable=False)
    description = Column(Text, nullable=True)
    price = Column(Numeric(15, 2), nullable=False)
    image_url = Column(String(255), nullable=True)
    
    # Inventory Management 
    is_available = Column(Boolean, default=True)
    stock_quantity = Column(Integer, nullable=True)
    
    created_at = Column(DateTime, default=datetime.utcnow)

    # Relationships
    category = relationship("Category", back_populates="items")
    # order_items relationship will be added in the Order module
