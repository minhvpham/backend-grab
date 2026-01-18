from pydantic import BaseModel, Field, field_validator, model_validator
from typing import Optional
from datetime import date
from app.models.promotion import DiscountType


class PromotionBase(BaseModel):
    name: str = Field(..., min_length=3, max_length=100)
    code: str = Field(..., min_length=3, max_length=20, pattern="^[A-Z0-9]+$")  # UPPERCASE only
    description: Optional[str] = None
    discount_type: DiscountType
    discount_value: float = Field(..., gt=0)
    min_order_value: float = Field(0, ge=0)
    max_discount_value: Optional[float] = None
    start_date: date
    end_date: date
    usage_limit: Optional[int] = Field(None, gt=0)


class PromotionCreate(PromotionBase):
    @field_validator('end_date')
    @classmethod
    def end_date_must_be_future(cls, v):
        from datetime import date as date_class
        if v <= date_class.today():
            raise ValueError('End date must be in the future')
        return v

    @model_validator(mode='after')
    def check_value_logic(self):
        # Logic: If Percentage, value must be 0-100
        if self.discount_type == DiscountType.PERCENTAGE and self.discount_value > 100:
            raise ValueError('Percentage discount cannot exceed 100%')
            
        # Logic: End Date > Start Date
        if self.end_date < self.start_date:
            raise ValueError('End date must be after start date')
            
        return self


class PromotionUpdate(BaseModel):
    # All fields optional for update
    description: Optional[str] = None
    is_active: Optional[bool] = None
    end_date: Optional[date] = None
    usage_limit: Optional[int] = None


class PromotionResponse(PromotionBase):
    id: int
    restaurant_id: int
    is_active: bool
    used_count: int
    
    class Config:
        from_attributes = True
