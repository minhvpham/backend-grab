import httpx
import os
from typing import Optional, Dict, Any
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

    async def create_trip(
        self,
        driver_id: str,
        order_id: str,
        pickup_address: str,
        delivery_address: str,
        pickup_latitude: float = None,
        pickup_longitude: float = None,
        delivery_latitude: float = None,
        delivery_longitude: float = None,
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

                response = await client.post(f"{self.base_url}/api/Trips", json=payload)
                response.raise_for_status()
                return response.json()
        except httpx.HTTPError as e:
            print(f"Error creating trip: {e}")
            return None

    async def initiate_driver_assignment(
        self,
        order_id: str,
        pickup_address: str,
        pickup_lat: Optional[float],
        pickup_lng: Optional[float],
        delivery_address: str,
        delivery_lat: Optional[float],
        delivery_lng: Optional[float],
        fare: float,
        customer_notes: Optional[str],
    ) -> bool:
        """
        Initiate driver assignment process for an order by creating a trip.
        This sends a request to Driver Service to create a new trip.

        Args:
            order_id: ID of the order to assign
            pickup_address: Restaurant address for pickup
            pickup_lat: Restaurant latitude
            pickup_lng: Restaurant longitude
            delivery_address: Customer delivery address
            delivery_lat: Delivery latitude
            delivery_lng: Delivery longitude
            fare: Calculated fare for the trip
            customer_notes: Customer delivery notes

        Returns:
            True if request was sent successfully, False otherwise
        """
        try:
            async with httpx.AsyncClient(timeout=TIMEOUT) as client:
                payload = {
                    "orderId": order_id,
                    "pickupAddress": pickup_address,
                    "deliveryAddress": delivery_address,
                    "fare": fare,
                    "customerNotes": customer_notes or "",
                }

                # Add coordinates if available
                if pickup_lat is not None and pickup_lng is not None:
                    payload["pickupLatitude"] = float(pickup_lat)
                    payload["pickupLongitude"] = float(pickup_lng)

                if delivery_lat is not None and delivery_lng is not None:
                    payload["deliveryLatitude"] = float(delivery_lat)
                    payload["deliveryLongitude"] = float(delivery_lng)

                response = await client.post(f"{self.base_url}/api/Trips", json=payload)
                response.raise_for_status()
                return True
        except httpx.HTTPError as e:
            print(f"Error initiating driver assignment: {e}")
            return False


# Singleton instance
driver_client = DriverServiceClient()

async def initiate_driver_assignment(
    order_id: str,
    pickup_address: str,
    pickup_lat: Optional[float] = None,
    pickup_lng: Optional[float] = None,
    delivery_address: str = "",
    delivery_lat: Optional[float] = None,
    delivery_lng: Optional[float] = None,
    fare: float = 0.0,
    customer_notes: Optional[str] = None,
) -> bool:
    """Initiate driver assignment process for an order by creating a trip"""
    return await driver_client.initiate_driver_assignment(
        order_id,
        pickup_address,
        pickup_lat,
        pickup_lng,
        delivery_address,
        delivery_lat,
        delivery_lng,
        fare,
        customer_notes,
    )
