from pydantic import BaseModel, EmailStr
from datetime import datetime
from models import UserStatusEnum, RoleEnum

class AdminUserResponse(BaseModel):
    id: int
    email: EmailStr
    role: RoleEnum
    status: UserStatusEnum
    is_deleted: bool
    created_at: datetime
    updated_at: datetime

    class Config:
        from_attributes = True
