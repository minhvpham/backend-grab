from fastapi import APIRouter
from app.api.endpoints import restaurants, menu

api_router = APIRouter()

api_router.include_router(restaurants.router, prefix="/restaurants", tags=["restaurants"])
api_router.include_router(menu.router, prefix="/menu", tags=["menu"])
