from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy.orm import Session
from typing import Optional
import hashlib

from ..database import get_db
from .. import schemas
from ..models import Profile
import uuid

router = APIRouter(
    prefix="/profiles",
    tags=["Profiles"],
    responses={404: {"description": "Not found"}}
)


def hash_password(password: str) -> str:
    """Hash password using SHA256"""
    return hashlib.sha256(password.encode()).hexdigest()


@router.post("/", response_model=schemas.ProfileSingleResponse, status_code=201)
def create_profile(profile: schemas.ProfileCreate, db: Session = Depends(get_db)):
    """Tạo profile mới"""
    # Check email exists
    existing = db.query(Profile).filter(Profile.email == profile.email).first()
    if existing:
        raise HTTPException(status_code=400, detail="Email đã tồn tại")
    
    # Check phone exists
    existing = db.query(Profile).filter(Profile.phone == profile.phone).first()
    if existing:
        raise HTTPException(status_code=400, detail="Số điện thoại đã tồn tại")
    # Check user_id exists
    existing = db.query(Profile).filter(Profile.user_id == profile.user_id).first()
    if existing:
        raise HTTPException(status_code=400, detail="User ID đã tồn tại")
    
    db_profile = Profile(
        user_id=profile.user_id,
        name=profile.name,
        email=profile.email,
        phone=profile.phone,
        password=hash_password(profile.password),
        role="user",  # Default role
        avatar=profile.avatar,
        address=profile.address
    )
    db.add(db_profile)
    db.commit()
    db.refresh(db_profile)
    
    return schemas.ProfileSingleResponse(
        success=True,
        message="Tạo profile thành công",
        data=db_profile
    )


@router.get("/", response_model=schemas.ProfileListResponse)
def read_profiles(
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=100),
    db: Session = Depends(get_db)
):
    """Lấy danh sách profiles"""
    profiles = db.query(Profile).offset(skip).limit(limit).all()
    total = db.query(Profile).count()
    
    return schemas.ProfileListResponse(
        success=True,
        message="Lấy danh sách profile thành công",
        data=profiles,
        total=total
    )


@router.get("/{user_id}", response_model=schemas.ProfileSingleResponse)
def read_profile(user_id: str, db: Session = Depends(get_db)):
    """Lấy thông tin profile theo user_id"""
    db_profile = db.query(Profile).filter(Profile.user_id == user_id).first()
    if not db_profile:
        raise HTTPException(status_code=404, detail="Profile không tồn tại")
    
    return schemas.ProfileSingleResponse(
        success=True,
        message="Lấy thông tin profile thành công",
        data=db_profile
    )





@router.put("/{user_id}", response_model=schemas.ProfileSingleResponse)
def update_profile(user_id: str, profile_update: schemas.ProfileUpdate, db: Session = Depends(get_db)):
    """Cập nhật thông tin profile"""
    db_profile = db.query(Profile).filter(Profile.user_id == user_id).first()
    if not db_profile:
        raise HTTPException(status_code=404, detail="Profile không tồn tại")
    
    update_data = profile_update.model_dump(exclude_unset=True)
    if "password" in update_data:
        update_data["password"] = hash_password(update_data["password"])
    if "role" in update_data and update_data["role"]:
        update_data["role"] = update_data["role"].value
    
    for field, value in update_data.items():
        setattr(db_profile, field, value)
    
    db.commit()
    db.refresh(db_profile)
    
    return schemas.ProfileSingleResponse(
        success=True,
        message="Cập nhật profile thành công",
        data=db_profile
    )


@router.delete("/{user_id}", response_model=schemas.MessageResponse)
def delete_profile(user_id: str, db: Session = Depends(get_db)):
    """Xóa profile"""
    db_profile = db.query(Profile).filter(Profile.user_id == user_id).first()
    if not db_profile:
        raise HTTPException(status_code=404, detail="Profile không tồn tại")
    
    db.delete(db_profile)
    db.commit()
    
    return schemas.MessageResponse(
        success=True,
        message="Xóa profile thành công"
    )
