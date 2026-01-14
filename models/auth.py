from sqlalchemy import Column, Integer, String, Boolean, Enum, DateTime, func
from database import Base
import enum

class RoleEnum(str, enum.Enum):
    user = "user"
    seller = "seller"
    shipper = "shipper"
    admin = "admin"

class UserStatusEnum(str, enum.Enum):
    active = "active"        # Tài khoản hoạt động bình thường
    inactive = "inactive"    # Bị admin vô hiệu hóa
    pending = "pending"      # Chờ xác minh (email, KYC, ...)
    banned = "banned"        # Bị cấm vĩnh viễn

class User(Base):
    __tablename__ = "users"
    
    id = Column(Integer, primary_key=True, index=True)
    email = Column(String, unique=True, index=True, nullable=False)
    hashed_password = Column(String, nullable=False)
    role = Column(Enum(RoleEnum), default=RoleEnum.user)
    status = Column(
        Enum(UserStatusEnum, name="user_status_enum"),
        default=UserStatusEnum.active,
        nullable=False,
    )
    is_deleted = Column(Boolean, default=False)
    
    created_at = Column(DateTime(timezone=True), server_default=func.now())
    updated_at = Column(DateTime(timezone=True), onupdate=func.now(), server_default=func.now())
