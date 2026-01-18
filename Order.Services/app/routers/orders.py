from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy.orm import Session
from typing import Optional

from ..database import get_db
from .. import crud, schemas
from ..services import driver_service, restaurant_service

router = APIRouter(
    prefix="/orders",
    tags=["Orders"],
    responses={404: {"description": "Not found"}}
)


@router.post("/", response_model=schemas.OrderSingleResponse, status_code=201)
async def create_order(order: schemas.OrderCreate, db: Session = Depends(get_db)):
    """
    Tạo đơn hàng mới

    - **profile_id**: ID của profile đặt hàng
    - **restaurant_id**: ID của nhà hàng
    - **items**: Danh sách món ăn
    - **delivery_address**: Địa chỉ giao hàng
    """
    created_order = crud.create_order(db=db, order=order)

    return schemas.OrderSingleResponse(
        success=True,
        message="Tạo đơn hàng thành công",
        data=created_order
    )


@router.get("/", response_model=schemas.OrderListResponse)
def read_orders(
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=100),
    db: Session = Depends(get_db)
):
    """
    Lấy danh sách tất cả đơn hàng
    """
    orders = crud.get_orders(db, skip=skip, limit=limit)
    total = crud.get_orders_count(db)
    
    return schemas.OrderListResponse(
        success=True,
        message="Lấy danh sách đơn hàng thành công",
        data=orders,
        total=total
    )


@router.get("/{order_id}", response_model=schemas.OrderSingleResponse)
def read_order(order_id: str, db: Session = Depends(get_db)):
    """
    Lấy chi tiết đơn hàng
    """
    db_order = crud.get_order(db, order_id=order_id)
    if db_order is None:
        raise HTTPException(status_code=404, detail="Đơn hàng không tồn tại")
    
    return schemas.OrderSingleResponse(
        success=True,
        message="Lấy thông tin đơn hàng thành công",
        data=db_order
    )


@router.put("/{order_id}", response_model=schemas.OrderSingleResponse)
async def update_order(
    order_id: str, order_update: schemas.OrderUpdate, db: Session = Depends(get_db)
):
    """
    Cập nhật đơn hàng (trạng thái, driver, địa chỉ...)
    Status-driven endpoint that applies business logic based on status transitions.
    """
    db_order = crud.get_order(db, order_id=order_id)
    if db_order is None:
        raise HTTPException(status_code=404, detail="Đơn hàng không tồn tại")

    # Import here to avoid circular import issues
    try:
        from ..services.status_handler import StatusHandler
    except ImportError:
        # Fallback if import fails
        StatusHandler = None

    # Validate status transition if status is being updated
    if order_update.status and StatusHandler:
        handler = StatusHandler(db)
        if not handler.validate_status_transition(db_order.status, order_update.status):
            raise HTTPException(
                status_code=400,
                detail=f"Invalid status transition from {db_order.status} to {order_update.status.value}",
            )

    updated_order = crud.update_order(
        db=db, order_id=order_id, order_update=order_update
    )

    # Apply status-driven logic if status was updated
    if order_update.status and StatusHandler:
        handler = StatusHandler(db)
        await handler.handle_status_update(updated_order, order_update.status)

    return schemas.OrderSingleResponse(
        success=True,
        message="Cập nhật đơn hàng thành công",
        data=updated_order
    )


@router.delete("/{order_id}", response_model=schemas.MessageResponse)
def delete_order(order_id: str, db: Session = Depends(get_db)):
    """
    Xóa đơn hàng
    """
    success = crud.delete_order(db=db, order_id=order_id)
    if not success:
        raise HTTPException(status_code=404, detail="Đơn hàng không tồn tại")
    
    return schemas.MessageResponse(
        success=True,
        message="Xóa đơn hàng thành công"
    )


@router.post("/{order_id}/cancel", response_model=schemas.OrderSingleResponse)
def cancel_order(order_id: str, db: Session = Depends(get_db)):
    """
    Hủy đơn hàng
    """
    db_order = crud.cancel_order(db=db, order_id=order_id)
    if db_order is None:
        raise HTTPException(status_code=400, detail="Không thể hủy đơn hàng này")
    
    return schemas.OrderSingleResponse(
        success=True,
        message="Hủy đơn hàng thành công",
        data=db_order
    )


# Get orders by user
@router.get("/user/{user_id}", response_model=schemas.OrderListResponse)
def read_user_orders(
    user_id: str,
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=100),
    db: Session = Depends(get_db)
):
    """
    Lấy lịch sử mua hàng của user
    """
    orders = crud.get_orders_by_user(db, user_id=user_id, skip=skip, limit=limit)
    total = crud.get_orders_count_by_user(db, user_id=user_id)
    
    return schemas.OrderListResponse(
        success=True,
        message="Lấy lịch sử đơn hàng thành công",
        data=orders,
        total=total
    )


# Get orders by driver
@router.get("/driver/{driver_id}", response_model=schemas.OrderListResponse)
def read_driver_orders(
    driver_id: str,
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=100),
    db: Session = Depends(get_db)
):
    """
    Lấy danh sách đơn hàng của driver
    """
    orders = crud.get_orders_by_driver(db, driver_id=driver_id, skip=skip, limit=limit)
    
    return schemas.OrderListResponse(
        success=True,
        message="Lấy danh sách đơn hàng driver thành công",
        data=orders,
        total=len(orders)
    )


# Get orders by restaurant
@router.get("/restaurant/{restaurant_id}", response_model=schemas.OrderListResponse)
def read_restaurant_orders(
    restaurant_id: str,
    skip: int = Query(0, ge=0),
    limit: int = Query(100, ge=1, le=100),
    db: Session = Depends(get_db)
):
    """
    Lấy danh sách đơn hàng của nhà hàng
    """
    orders = crud.get_orders_by_restaurant(db, restaurant_id=restaurant_id, skip=skip, limit=limit)
    
    return schemas.OrderListResponse(
        success=True,
        message="Lấy danh sách đơn hàng nhà hàng thành công",
        data=orders,
        total=len(orders)
    )
