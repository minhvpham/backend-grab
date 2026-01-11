from pydantic import BaseModel, EmailStr
from typing import List


class AdminUserResponse(BaseModel):
    id: int
    email: EmailStr
    role: str
    is_active: bool

    class Config:
        from_attributes = True