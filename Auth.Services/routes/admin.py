import os
import httpx
from fastapi import APIRouter, Depends, Query, HTTPException
from sqlalchemy.orm import Session
from typing import List, Optional
from sqlalchemy import asc, desc

from database import get_db
from models import User, RoleEnum, UserStatusEnum
from schemas.admin import AdminUserResponse, UpdateUserStatusRequest
from utils import require_admin

router = APIRouter(
    prefix="/admin",
    tags=["Admin"]
)

DRIVER_SERVICE_URL = os.getenv("DRIVER_SERVICE_URL", None)
RESTAURANT_SERVICE_URL = os.getenv("RESTAURANT_SERVICE_URL", None)

@router.patch("/users/{user_id}/status", response_model=AdminUserResponse)
async def update_user_status(
    user_id: int,
    payload: UpdateUserStatusRequest,
    db: Session = Depends(get_db),
    admin=Depends(require_admin),
):
    # ===== 1. Validate user =====
    user = db.query(User).filter(User.id == user_id).first()

    if not user:
        raise HTTPException(status_code=404, detail="User not found")

    if user.role == RoleEnum.admin:
        raise HTTPException(status_code=400, detail="Cannot modify admin account")

    if user.id == admin.id:
        raise HTTPException(status_code=400, detail="Cannot modify your own account")

    # ===== 2. Update status trong Auth Service =====
    user.status = payload.status
    db.commit()
    db.refresh(user)

    async with httpx.AsyncClient(timeout=5.0) as client:

        # ================= SELLER / RESTAURANT =================
        if user.role == RoleEnum.seller:
            if not RESTAURANT_SERVICE_URL:
                raise HTTPException(
                    status_code=500,
                    detail="RESTAURANT_SERVICE_URL not configured"
                )

            # ---- 2.1 Map status auth -> restaurant ----
            RESTAURANT_STATUS_MAP = {
                "pending": "PENDING",
                "active": "ACTIVE",
                "inactive": "REJECTED",
                "banned": "BANNED",
            }

            restaurant_status = RESTAURANT_STATUS_MAP.get(payload.status.value)
            if not restaurant_status:
                raise HTTPException(
                    status_code=400,
                    detail="Invalid status for restaurant"
                )

            # ---- 2.2 Get restaurant by owner_id ----
            get_restaurant_url = (
                f"{RESTAURANT_SERVICE_URL}"
                f"/api/v1/restaurants/owner/{user.id}"
            )

            get_response = await client.get(get_restaurant_url)

            if get_response.status_code != 200:
                raise HTTPException(
                    status_code=502,
                    detail="Failed to get restaurant by owner"
                )

            restaurants = get_response.json()
            if not restaurants:
                raise HTTPException(
                    status_code=404,
                    detail="Seller has no restaurant"
                )

            # ⚠️ giả định: mỗi seller chỉ có 1 restaurant
            restaurant_id = restaurants[0]["id"]

            # ---- 2.3 Update restaurant status ----
            patch_url = (
                f"{RESTAURANT_SERVICE_URL}"
                f"/api/v1/restaurants/{restaurant_id}/status"
            )

            patch_response = await client.patch(
                patch_url,
                params={"new_status": restaurant_status}
            )

            if patch_response.status_code != 200:
                raise HTTPException(
                    status_code=502,
                    detail=f"Failed to update restaurant status: {patch_response.text}"
                )

        # ================= DRIVER =================
        elif user.role == RoleEnum.shipper:
            if not DRIVER_SERVICE_URL:
                raise HTTPException(
                    status_code=500,
                    detail="DRIVER_SERVICE_URL not configured"
                )

            if payload.status == UserStatusEnum.active:
                url = f"{DRIVER_SERVICE_URL}/api/Drivers/{user.id}/verify"
                response = await client.post(url)

            elif payload.status == UserStatusEnum.reject:
                url = f"{DRIVER_SERVICE_URL}/api/Drivers/{user.id}/reject"
                response = await client.post(
                    url,
                    json={"reason": "Rejected by admin"}
                )
            else:
                response = None

            if response and response.status_code not in (200, 204):
                raise HTTPException(
                    status_code=502,
                    detail="Failed to update driver status"
                )

    return user

@router.get("/users", response_model=List[AdminUserResponse])
def list_users(
    role: Optional[RoleEnum] = Query(None, description="Filter by role"),
    status: Optional[UserStatusEnum] = Query(None, description="Filter by status: active, inactive"),
    is_deleted: Optional[bool] = Query(False, description="Include deleted users"),
    sort_by: Optional[str] = Query("created_at", description="Sort by: created_at or updated_at"),
    order: Optional[str] = Query("desc", description="Order: asc or desc"),
    db: Session = Depends(get_db),
    admin: User =Depends(require_admin)
):
    query = db.query(User).filter(User.id != admin.id)

    if role is not None:
        query = query.filter(User.role == role)

    if status is not None:
        query = query.filter(User.status == status)

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
    user.status = UserStatusEnum.banned
    db.commit()
    db.refresh(user)
    return user

@router.get("/users/{user_id}", response_model=AdminUserResponse)
def get_user_detail(
    user_id: int,
    db: Session = Depends(get_db),
    admin: User = Depends(require_admin),
):
    user = (
        db.query(User)
        .filter(
            User.id == user_id,
            User.id != admin.id,
            User.is_deleted == False
        )
        .first()
    )

    if not user:
        raise HTTPException(
            status_code=404,
            detail="User not found"
        )

    return user