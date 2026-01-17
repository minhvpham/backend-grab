"""
Seed script to create initial test data in the database.
Run this after running migrations to seed default global categories.
"""
from app.db.base import SessionLocal
from app.models.menu import Category
from sqlalchemy.exc import IntegrityError


def seed_default_categories():
    """Create default global categories"""
    db = SessionLocal()
    
    # Default categories shared across all restaurants
    default_categories = [
        {"name": "Đại hạ giá"},
        {"name": "Ăn vặt"},
        {"name": "Ăn trưa"},
        {"name": "Đồ uống"},
    ]
    
    created_count = 0
    
    for category_data in default_categories:
        try:
            # Check if already exists
            existing = db.query(Category).filter(
                Category.name == category_data["name"]
            ).first()
            
            if not existing:
                category = Category(**category_data)
                db.add(category)
                db.commit()
                db.refresh(category)
                print(f"✓ Created category: {category.name} (ID: {category.id})")
                created_count += 1
            else:
                print(f"✓ Category '{category_data['name']}' already exists")
        except IntegrityError:
            db.rollback()
            print(f"✗ Category '{category_data['name']}' already exists, skipping...")
        except Exception as e:
            db.rollback()
            print(f"✗ Error creating category {category_data['name']}: {e}")
    
    db.close()
    print(f"\n✓ Seed completed! Created {created_count} new categories.")
    print("Global categories ready: Đại hạ giá, Ăn vặt, Ăn trưa, Đồ uống")


if __name__ == "__main__":
    print("Starting database seeding...\n")
    seed_default_categories()
    print("\nGlobal categories are ready to be used by all restaurants.")
