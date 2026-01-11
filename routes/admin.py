from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from typing import List

from database import get_db
from models import User, RoleEnum
from schemas.admin import AdminUserResponse
from utils import require_admin

router = APIRouter(
    prefix="/admin",
    tags=["Admin"]
)

@router.patch("/users/{user_id}/activate", response_model=AdminUserResponse)
def activate_user(
    user_id: int,
    db: Session = Depends(get_db),
    admin=Depends(require_admin)
):
    user = db.query(User).filter(User.id == user_id).first()

    if not user:
        raise HTTPException(status_code=404, detail="User not found")

    if user.role == RoleEnum.admin:
        raise HTTPException(status_code=400, detail="Cannot modify admin")

    user.is_active = True
    db.commit()
    db.refresh(user)

    return user

@router.patch("/users/{user_id}/block", response_model=AdminUserResponse)
def block_user(
    user_id: int,
    db: Session = Depends(get_db),
    admin=Depends(require_admin)
):
    user = db.query(User).filter(User.id == user_id).first()

    if not user:
        raise HTTPException(status_code=404, detail="User not found")

    if user.role == RoleEnum.admin:
        raise HTTPException(status_code=400, detail="Cannot block admin")

    user.is_active = False
    db.commit()
    db.refresh(user)

    return user
