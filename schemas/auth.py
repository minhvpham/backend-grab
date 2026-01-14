from pydantic import BaseModel, EmailStr
from typing import Literal
from models import RoleEnum, UserStatusEnum

class UserCreate(BaseModel):
    email: EmailStr
    password: str
    role: RoleEnum

class UserResponse(BaseModel):
    id: int
    email: EmailStr
    role: RoleEnum
    status: UserStatusEnum

    class Config:
        orm_mode = True


class LoginRequest(BaseModel):
    email: str
    password: str

class Token(BaseModel):
    access_token: str
    token_type: str
