import os
import uuid
from pathlib import Path
from fastapi import UploadFile

# Base upload directory
UPLOAD_DIR = Path("uploads")
BUSINESS_LICENSE_DIR = UPLOAD_DIR / "business_licenses"
FOOD_SAFETY_CERT_DIR = UPLOAD_DIR / "food_safety_certificates"

# Allowed image extensions
ALLOWED_EXTENSIONS = {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"}


def ensure_upload_directories():
    """Create upload directories if they don't exist"""
    BUSINESS_LICENSE_DIR.mkdir(parents=True, exist_ok=True)
    FOOD_SAFETY_CERT_DIR.mkdir(parents=True, exist_ok=True)


def is_allowed_image(filename: str) -> bool:
    """Check if the file has an allowed image extension"""
    ext = Path(filename).suffix.lower()
    return ext in ALLOWED_EXTENSIONS


def generate_unique_filename(original_filename: str) -> str:
    """Generate a unique filename while preserving the extension"""
    ext = Path(original_filename).suffix.lower()
    unique_name = f"{uuid.uuid4()}{ext}"
    return unique_name


async def save_upload_file(upload_file: UploadFile, destination_dir: Path) -> str:
    """
    Save an uploaded file to the specified directory
    
    Args:
        upload_file: The FastAPI UploadFile object
        destination_dir: Directory to save the file
        
    Returns:
        The relative path to the saved file
        
    Raises:
        ValueError: If file type is not allowed
    """
    if not is_allowed_image(upload_file.filename):
        raise ValueError(f"File type not allowed. Allowed types: {', '.join(ALLOWED_EXTENSIONS)}")
    
    # Generate unique filename
    unique_filename = generate_unique_filename(upload_file.filename)
    file_path = destination_dir / unique_filename
    
    # Ensure directory exists
    destination_dir.mkdir(parents=True, exist_ok=True)
    
    # Save file
    content = await upload_file.read()
    with open(file_path, "wb") as f:
        f.write(content)
    
    # Return relative path for database storage
    return str(file_path.relative_to(UPLOAD_DIR.parent))


async def save_business_license(upload_file: UploadFile) -> str:
    """Save business license image and return its path"""
    return await save_upload_file(upload_file, BUSINESS_LICENSE_DIR)


async def save_food_safety_certificate(upload_file: UploadFile) -> str:
    """Save food safety certificate image and return its path"""
    return await save_upload_file(upload_file, FOOD_SAFETY_CERT_DIR)


def delete_file(file_path: str) -> bool:
    """Delete a file from the filesystem"""
    try:
        full_path = Path(file_path)
        if full_path.exists():
            full_path.unlink()
            return True
        return False
    except Exception as e:
        print(f"Error deleting file {file_path}: {e}")
        return False
