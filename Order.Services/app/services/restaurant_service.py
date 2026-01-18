
import httpx
import os
from typing import Optional, Dict, Any
from dataclasses import dataclass

# Config
RESTAURANT_SERVICE_URL = os.getenv("RESTAURANT_SERVICE_URL", "http://restaurants-service:8080")
TIMEOUT = 10.0

@dataclass
class RestaurantInfo:
    id: str
    name: str
    address: str
    phone: Optional[str] = None

class RestaurantServiceClient:
    def __init__(self, base_url: str = None):
        self.base_url = base_url or RESTAURANT_SERVICE_URL

    async def get_restaurant_info(self, restaurant_id: str) -> Optional[RestaurantInfo]:
        try:
            async with httpx.AsyncClient(timeout=TIMEOUT) as client:
                response = await client.get(f"{self.base_url}/api/Restaurants/{restaurant_id}")
                response.raise_for_status()
                data = response.json()
                return RestaurantInfo(
                    id=str(data.get("id")),
                    name=data.get("name"),
                    address=data.get("address"),
                    phone=data.get("phone")
                )
        except httpx.HTTPError as e:
            print(f"Error getting restaurant info: {e}")
            return None

    async def evaluate_order(
        self, order_id: str, restaurant_id: str, order_data: Dict[str, Any]
    ) -> bool:
        """Send order evaluation request to restaurant service"""
        try:
            async with httpx.AsyncClient(timeout=TIMEOUT) as client:
                payload = {
                    "order_id": order_id,
                    "restaurant_id": restaurant_id,
                    "order_data": order_data,
                }
                response = await client.post(
                    f"{self.base_url}/api/orders/evaluate", json=payload
                )
                response.raise_for_status()
                return True
        except httpx.HTTPError as e:
            print(f"Error sending restaurant evaluation: {e}")
            return False


restaurant_client = RestaurantServiceClient()

async def get_restaurant_details(restaurant_id: str) -> Optional[RestaurantInfo]:
    return await restaurant_client.get_restaurant_info(restaurant_id)


async def evaluate_order(
    order_id: str, restaurant_id: str, order_data: Dict[str, Any]
) -> bool:
    """Send order evaluation request to restaurant service"""
    return await restaurant_client.evaluate_order(order_id, restaurant_id, order_data)
