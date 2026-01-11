from sqlalchemy import Column, Integer, String, Boolean, Enum
from database import Base
import enum

class RoleEnum(str, enum.Enum):
    user = "user"
    seller = "seller"
    shipper = "shipper"
    admin = "admin"

class User(Base):
    __tablename__ = "users"
    
    id = Column(Integer, primary_key=True, index=True)
    email = Column(String, unique=True, index=True, nullable=False)
    hashed_password = Column(String, nullable=False)
    role = Column(Enum(RoleEnum), default=RoleEnum.user)
    is_active = Column(Boolean, default=True)  # mặc định True cho user, pending cho seller/shipper
