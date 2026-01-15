from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker, DeclarativeBase
from dotenv import load_dotenv
import os

load_dotenv()

DATABASE_URL = os.getenv("DATABASE_URL")

engine = create_engine(DATABASE_URL)

SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

# 3. Create the Base class
# All your models (like Restaurant) will inherit from this
class Base(DeclarativeBase):
    pass

# 4. Dependency function to get DB session in API endpoints
def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()