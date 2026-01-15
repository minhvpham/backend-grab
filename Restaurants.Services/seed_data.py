"""
Seed script to create initial test data in the database.
Run this after running migrations to create test users.
"""
from app.db.base import SessionLocal
from app.models.auth import User, RoleEnum
from sqlalchemy.exc import IntegrityError


def create_test_users():
    """Create test users for development"""
    db = SessionLocal()
    
    test_users = [
        {
            "id": 1,
            "email": "seller1@example.com",
            "hashed_password": "hashed_password_placeholder",  # In production, hash this properly
            "role": RoleEnum.seller,
            "is_active": True,
            "is_deleted": False
        },
        {
            "id": 2,
            "email": "seller2@example.com",
            "hashed_password": "hashed_password_placeholder",
            "role": RoleEnum.seller,
            "is_active": True,
            "is_deleted": False
        },
        {
            "id": 3,
            "email": "admin@example.com",
            "hashed_password": "hashed_password_placeholder",
            "role": RoleEnum.admin,
            "is_active": True,
            "is_deleted": False
        },
        {
            "id": 4,
            "email": "user@example.com",
            "hashed_password": "hashed_password_placeholder",
            "role": RoleEnum.user,
            "is_active": True,
            "is_deleted": False
        }
    ]
    
    created_count = 0
    for user_data in test_users:
        try:
            user = User(**user_data)
            db.add(user)
            db.commit()
            db.refresh(user)
            print(f"✓ Created user: {user.email} (ID: {user.id}, Role: {user.role})")
            created_count += 1
        except IntegrityError:
            db.rollback()
            print(f"✗ User {user_data['email']} already exists, skipping...")
        except Exception as e:
            db.rollback()
            print(f"✗ Error creating user {user_data['email']}: {e}")
    
    db.close()
    print(f"\n✓ Seed completed! Created {created_count} new users.")


if __name__ == "__main__":
    print("Starting database seeding...\n")
    create_test_users()
    print("\nYou can now create restaurants with owner_id=1 (seller1@example.com)")
