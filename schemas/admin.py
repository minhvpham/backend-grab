from pydantic import BaseModel, EmailStr
from typing import List, Optional
from datetime import datetime

class AdminUserResponse(BaseModel):
    id: int
    email: EmailStr
    role: str
    is_active: bool
    is_deleted: bool
    created_at: datetime
    updated_at: datetime

    class Config:
        from_attributes = True
