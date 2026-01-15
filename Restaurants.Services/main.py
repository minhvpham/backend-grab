from fastapi import FastAPI
from app.api import api_router
from app.db.base import Base, engine
from fastapi.middleware.cors import CORSMiddleware

# Create database tables
# Note: In production, use Alembic migrations instead
# Base.metadata.create_all(bind=engine)

app = FastAPI(
    title="Restaurant Service API",
    description="API for managing restaurants in the Grab-like food delivery system",
    version="1.0.0"
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "http://localhost:3000",   # React / Next.js local
        "http://localhost:5173",   # Vite
    ],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


# Include API routes
app.include_router(api_router, prefix="/api/v1")


@app.get("/")
def root():
    """
    Root endpoint - API health check.
    """
    return {
        "message": "Restaurant Service API is running",
        "version": "1.0.0",
        "docs": "/docs"
    }


@app.get("/health")
def health_check():
    """
    Health check endpoint.
    """
    return {"status": "healthy"}


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8080)
