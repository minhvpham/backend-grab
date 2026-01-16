from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy.orm import Session
from typing import Optional
import hashlib

from ..database import get_db
from .. import schemas
from ..models import User
import uuid

router = APIRouter(
    prefix="/users",
    tags=["Users"],
    responses={404: {"description": "Not found"}}
)


def hash_password(password: str) -> str:
    """Hash password using SHA256"""
    return hashlib.sha256(password.encode()).hexdigest()


@router.post("/", response_model=schemas.UserSingleResponse, status_code=201)
def create_user(user: schemas.UserCreate, db: Session = Depends(get_db)):
    """Tạo user mới"""
    # Check email exists
    existing = db.query(User).filter(User.email == user.email).first()
    if existing:
        raise HTTPException(status_code=400, detail="Email đã tồn tại")
    
    # Check phone exists
    existing = db.query(User).filter(User.phone == user.phone).first()
    if existing:
        raise HTTPException(status_code=400, detail="Số điện thoại đã tồn tại")
    
    db_user = User(
        name=user.name,
        email=user.email,
        phone=user.phone,
        password=hash_password(user.password),
        role=user.role.value,
        avatar=user.avatar,
        address=user.address
    )
    db.add(db_user)
    db.commit()
    db.refresh(db_user)
    
    return schemas.UserSingleResponse(
        success=True,
        message="Tạo user thành công",
        data=db_user
    )


@router.get("/", response_model=schemas.UserListResponse)
def read_users(
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=100),
    db: Session = Depends(get_db)
):
    """Lấy danh sách users"""
    users = db.query(User).offset(skip).limit(limit).all()
    total = db.query(User).count()
    
    return schemas.UserListResponse(
        success=True,
        message="Lấy danh sách user thành công",
        data=users,
        total=total
    )


@router.get("/{user_id}", response_model=schemas.UserSingleResponse)
def read_user(user_id: str, db: Session = Depends(get_db)):
    """Lấy thông tin user"""
    db_user = db.query(User).filter(User.id == uuid.UUID(user_id)).first()
    if not db_user:
        raise HTTPException(status_code=404, detail="User không tồn tại")
    
    return schemas.UserSingleResponse(
        success=True,
        message="Lấy thông tin user thành công",
        data=db_user
    )


@router.put("/{user_id}", response_model=schemas.UserSingleResponse)
def update_user(user_id: str, user_update: schemas.UserUpdate, db: Session = Depends(get_db)):
    """Cập nhật thông tin user"""
    db_user = db.query(User).filter(User.id == uuid.UUID(user_id)).first()
    if not db_user:
        raise HTTPException(status_code=404, detail="User không tồn tại")
    
    update_data = user_update.model_dump(exclude_unset=True)
    if "password" in update_data:
        update_data["password"] = hash_password(update_data["password"])
    if "role" in update_data and update_data["role"]:
        update_data["role"] = update_data["role"].value
    
    for field, value in update_data.items():
        setattr(db_user, field, value)
    
    db.commit()
    db.refresh(db_user)
    
    return schemas.UserSingleResponse(
        success=True,
        message="Cập nhật user thành công",
        data=db_user
    )


@router.delete("/{user_id}", response_model=schemas.MessageResponse)
def delete_user(user_id: str, db: Session = Depends(get_db)):
    """Xóa user"""
    db_user = db.query(User).filter(User.id == uuid.UUID(user_id)).first()
    if not db_user:
        raise HTTPException(status_code=404, detail="User không tồn tại")
    
    db.delete(db_user)
    db.commit()
    
    return schemas.MessageResponse(
        success=True,
        message="Xóa user thành công"
    )
