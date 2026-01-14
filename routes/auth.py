import os

from fastapi import Depends, HTTPException, status, APIRouter
from sqlalchemy.orm import Session

from database import get_db
from models import User, RoleEnum, UserStatusEnum
from schemas import UserCreate, UserResponse, Token, LoginRequest

from utils import (
    hash_password,
    verify_password,
    create_access_token,
    get_current_user,
)

ACCESS_TOKEN_EXPIRE_MINUTES = int(os.getenv("ACCESS_TOKEN_EXPIRE_MINUTES", 30))

router = APIRouter(
    prefix="/auth",
    tags=["Authentication"]
)


@router.post("/register", response_model=UserResponse)
def register(user: UserCreate, db: Session = Depends(get_db)):
    # 1. Check email tồn tại
    existing_user = db.query(User).filter(User.email == user.email).first()
    if existing_user:
        raise HTTPException(status_code=400, detail="Email already exists")

    # 2. Không cho tạo admin
    if user.role == RoleEnum.admin:
        raise HTTPException(
            status_code=400,
            detail="Cannot register admin accounts"
        )

    # 3. Set status theo role
    if user.role == RoleEnum.user:
        status_value = UserStatusEnum.active
    else:
        status_value = UserStatusEnum.pending

    # 4. Tạo user
    new_user = User(
        email=user.email,
        hashed_password=hash_password(user.password),
        role=user.role,
        status=status_value,
    )

    db.add(new_user)
    db.commit()
    db.refresh(new_user)

    return new_user


@router.post("/login/{user_role}", response_model=Token)
def login(
    user_role: RoleEnum,
    data: LoginRequest,
    db: Session = Depends(get_db)
):
    user = db.query(User).filter(User.email == data.email).first()

    # 1. Check credentials
    if not user or not verify_password(data.password, user.hashed_password):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Incorrect email or password"
        )

    # 2. Check role
    if user.role != user_role:
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail=f"User is not allowed to login as {user_role}"
        )

    # 3. Check status
    if user.status != UserStatusEnum.active:
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail=f"Account status is '{user.status}', login not allowed"
        )

    # 4. Create token
    access_token = create_access_token(
        data={
            "sub": str(user.id),
            "role": user.role,
        }
    )

    return {
        "access_token": access_token,
        "token_type": "bearer",
    }


@router.get("/me", response_model=UserResponse)
def get_me(current_user: User = Depends(get_current_user)):
    return current_user
