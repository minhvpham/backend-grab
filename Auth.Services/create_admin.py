from dotenv import load_dotenv

load_dotenv()
import os
from database import SessionLocal
from models import User, RoleEnum
from utils import hash_password  # hoặc hàm hash_password bạn đang dùng

ADMIN_EMAIL = os.getenv("ADMIN_EMAIL")
ADMIN_PASSWORD=os.getenv("ADMIN_PASSWORD")

def create_admin():
    db = SessionLocal()

    email = ADMIN_EMAIL
    password = ADMIN_PASSWORD

    existing = db.query(User).filter(User.email == email).first()
    if existing:
        print("Admin already exists")
        return

    admin = User(
        email=email,
        hashed_password=hash_password(password),
        role=RoleEnum.admin,
        is_active=True
    )

    db.add(admin)
    db.commit()
    db.close()

    print("✅ Admin account created")
    print(f"Email: {email}")
    print(f"Password: {password}")

if __name__ == "__main__":
    create_admin()
