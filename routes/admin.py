from fastapi import APIRouter, Depends, Query, HTTPException
from sqlalchemy.orm import Session
from typing import List, Optional
from sqlalchemy import asc, desc

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

@router.get("/users", response_model=List[AdminUserResponse])
def list_users(
    role: Optional[RoleEnum] = Query(None, description="Filter by role"),
    status: Optional[str] = Query(None, description="Filter by status: active, inactive"),
    is_deleted: Optional[bool] = Query(False, description="Include deleted users"),
    sort_by: Optional[str] = Query("created_at", description="Sort by: created_at or updated_at"),
    order: Optional[str] = Query("desc", description="Order: asc or desc"),
    db: Session = Depends(get_db),
    admin: User =Depends(require_admin)
):
    query = db.query(User).filter(User.id != admin.id)

    if role:
        query = query.filter(User.role == role)

    if status == "active":
        query = query.filter(User.is_active == True)
    elif status == "inactive":
        query = query.filter(User.is_active == False)

    query = query.filter(User.is_deleted == is_deleted)

    # sort
    if sort_by not in ["created_at", "updated_at"]:
        sort_by = "created_at"
    sort_column = getattr(User, sort_by)
    if order == "asc":
        query = query.order_by(asc(sort_column))
    else:
        query = query.order_by(desc(sort_column))

    users = query.all()
    return users

@router.patch("/users/{user_id}/delete", response_model=AdminUserResponse)
def soft_delete_user(
    user_id: int,
    db: Session = Depends(get_db),
    admin=Depends(require_admin)
):
    user = db.query(User).filter(User.id == user_id).first()
    if not user:
        raise HTTPException(status_code=404, detail="User not found")
    if user.role == RoleEnum.admin:
        raise HTTPException(status_code=400, detail="Cannot delete admin")

    user.is_deleted = True
    user.is_active = False
    db.commit()
    db.refresh(user)
    return user
