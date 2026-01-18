"""
Driver Service Client - Gọi API từ Driver Service

APIs sử dụng:
1. GET /api/DriverLocations/nearby - Tìm tài xế online gần đây
2. GET /api/Drivers/{id} - Lấy thông tin tài xế
3. GET /api/DriverLocations/{id} - Lấy vị trí GPS tài xế
4. POST /api/Trips - Tạo chuyến đi (gán đơn cho tài xế)
"""

import httpx
import os
from typing import Optional, List, Dict, Any
from dataclasses import dataclass


# Config
DRIVER_SERVICE_URL = os.getenv("DRIVER_SERVICE_URL", "http://driver-service:8081")
TIMEOUT = 10.0  # seconds


@dataclass
class DriverInfo:
    """Thông tin tài xế"""
    id: str
    full_name: str
    phone_number: str
    email: Optional[str] = None
    vehicle_type: Optional[str] = None
    license_plate: Optional[str] = None
    status: Optional[str] = None


@dataclass
class DriverLocation:
    """Vị trí tài xế"""
    driver_id: str
    latitude: float
    longitude: float
    updated_at: Optional[str] = None


class DriverServiceClient:
    """Client để gọi Driver Service API"""
    
    def __init__(self, base_url: str = None):
        self.base_url = base_url or DRIVER_SERVICE_URL
    
    async def get_nearby_drivers(
        self, 
        latitude: float, 
        longitude: float, 
        radius_km: float = 5.0
    ) -> List[Dict[str, Any]]:
        """
        Tìm tài xế online gần vị trí nhà hàng
        
        Args:
            latitude: Vĩ độ nhà hàng
            longitude: Kinh độ nhà hàng
            radius_km: Bán kính tìm kiếm (km)
        
        Returns:
            Danh sách tài xế gần đó
        """
        try:
            async with httpx.AsyncClient(timeout=TIMEOUT) as client:
                response = await client.get(
                    f"{self.base_url}/api/DriverLocations/nearby",
                    params={
                        "latitude": latitude,
                        "longitude": longitude,
                        "radiusKm": radius_km
                    }
                )
                response.raise_for_status()
                return response.json()
        except httpx.HTTPError as e:
            print(f"Error getting nearby drivers: {e}")
            return []
    
    async def get_driver_info(self, driver_id: str) -> Optional[DriverInfo]:
        """
        Lấy thông tin chi tiết tài xế (tên, SĐT, biển số xe)
        
        Args:
            driver_id: ID của tài xế
        
        Returns:
            DriverInfo hoặc None nếu không tìm thấy
        """
        try:
            async with httpx.AsyncClient(timeout=TIMEOUT) as client:
                response = await client.get(
                    f"{self.base_url}/api/Drivers/{driver_id}"
                )
                response.raise_for_status()
                data = response.json()
                return DriverInfo(
                    id=data.get("id"),
                    full_name=data.get("fullName", ""),
                    phone_number=data.get("phoneNumber", ""),
                    email=data.get("email"),
                    vehicle_type=data.get("vehicleType"),
                    license_plate=data.get("licensePlate"),
                    status=data.get("status")
                )
        except httpx.HTTPError as e:
            print(f"Error getting driver info: {e}")
            return None
    
    async def get_driver_location(self, driver_id: str) -> Optional[DriverLocation]:
        """
        Lấy vị trí GPS realtime của tài xế
        
        Args:
            driver_id: ID của tài xế
        
        Returns:
            DriverLocation hoặc None nếu không tìm thấy
        """
        try:
            async with httpx.AsyncClient(timeout=TIMEOUT) as client:
                response = await client.get(
                    f"{self.base_url}/api/DriverLocations/{driver_id}"
                )
                response.raise_for_status()
                data = response.json()
                return DriverLocation(
                    driver_id=driver_id,
                    latitude=data.get("latitude", 0),
                    longitude=data.get("longitude", 0),
                    updated_at=data.get("updatedAt")
                )
        except httpx.HTTPError as e:
            print(f"Error getting driver location: {e}")
            return None
    
    async def create_trip(
        self,
        driver_id: str,
        order_id: str,
        pickup_address: str,
        delivery_address: str,
        pickup_latitude: float = None,
        pickup_longitude: float = None,
        delivery_latitude: float = None,
        delivery_longitude: float = None
    ) -> Optional[Dict[str, Any]]:
        """
        Tạo chuyến đi - Gán đơn hàng cho tài xế
        
        Args:
            driver_id: ID tài xế
            order_id: ID đơn hàng
            pickup_address: Địa chỉ lấy hàng (nhà hàng)
            delivery_address: Địa chỉ giao hàng (khách)
        
        Returns:
            Thông tin trip hoặc None nếu thất bại
        """
        try:
            async with httpx.AsyncClient(timeout=TIMEOUT) as client:
                payload = {
                    "driverId": driver_id,
                    "orderId": order_id,
                    "pickupAddress": pickup_address,
                    "deliveryAddress": delivery_address,
                }
                
                if pickup_latitude and pickup_longitude:
                    payload["pickupLatitude"] = pickup_latitude
                    payload["pickupLongitude"] = pickup_longitude
                
                if delivery_latitude and delivery_longitude:
                    payload["deliveryLatitude"] = delivery_latitude
                    payload["deliveryLongitude"] = delivery_longitude
                
                response = await client.post(
                    f"{self.base_url}/api/Trips",
                    json=payload
                )
                response.raise_for_status()
                return response.json()
        except httpx.HTTPError as e:
            print(f"Error creating trip: {e}")
            return None

    async def initiate_driver_assignment(self, order_id: str) -> bool:
        """
        Initiate driver assignment process for an order.
        This sends a request to Driver Service to start the assignment workflow.

        Args:
            order_id: ID of the order to assign

        Returns:
            True if request was sent successfully, False otherwise
        """
        try:
            async with httpx.AsyncClient(timeout=TIMEOUT) as client:
                payload = {"order_id": order_id}
                response = await client.post(
                    f"{self.base_url}/api/orders/assign-driver", json=payload
                )
                response.raise_for_status()
                return True
        except httpx.HTTPError as e:
            print(f"Error initiating driver assignment: {e}")
            return False


# Singleton instance
driver_client = DriverServiceClient()


# Convenience functions
async def find_nearby_drivers(lat: float, lng: float, radius_km: float = 5.0):
    """Tìm tài xế gần vị trí"""
    return await driver_client.get_nearby_drivers(lat, lng, radius_km)


async def get_driver_details(driver_id: str):
    """Lấy thông tin tài xế"""
    return await driver_client.get_driver_info(driver_id)


async def get_driver_gps(driver_id: str):
    """Lấy vị trí GPS tài xế"""
    return await driver_client.get_driver_location(driver_id)


async def assign_order_to_driver(driver_id: str, order_id: str, pickup: str, delivery: str):
    """Gán đơn hàng cho tài xế"""
    return await driver_client.create_trip(driver_id, order_id, pickup, delivery)


async def initiate_driver_assignment(order_id: str) -> bool:
    """Initiate driver assignment process for an order"""
    return await driver_client.initiate_driver_assignment(order_id)
