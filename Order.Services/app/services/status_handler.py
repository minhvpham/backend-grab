"""
Status-driven order processing handlers.

This module contains the business logic for handling order status transitions
in a source-agnostic manner.
"""

from typing import Optional
from sqlalchemy.orm import Session
from .. import models, schemas
from . import driver_service


class StatusHandler:
    """Handles status-specific logic for order updates"""

    def __init__(self, db: Session):
        self.db = db

    async def handle_status_update(
        self, order: models.Order, new_status: schemas.OrderStatus
    ) -> None:
        """
        Apply status-specific logic based on the new status value.
        This method is called after the status has been updated in the database.
        """
        if new_status == schemas.OrderStatus.RESTAURANT_REJECTED:
            await self._handle_restaurant_rejected(order)
        elif new_status == schemas.OrderStatus.RESTAURANT_ACCEPTED:
            await self._handle_restaurant_accepted(order)
        elif new_status == schemas.OrderStatus.DRIVER_ACCEPTED:
            await self._handle_driver_accepted(order)
        elif new_status == schemas.OrderStatus.DRIVER_REJECTED:
            await self._handle_driver_rejected(order)

    async def _handle_restaurant_rejected(self, order: models.Order) -> None:
        """Handle restaurant rejection - mark as terminal state"""
        # No further actions needed - order is terminal
        pass

    async def _handle_restaurant_accepted(self, order: models.Order) -> None:
        """Handle restaurant acceptance - initiate driver assignment"""
        # Send request to Driver Services to initiate driver assignment
        # This is a fire-and-forget operation
        try:
            # Get restaurant information for pickup details
            from . import restaurant_service

            restaurant_id = str(order.restaurant_id)
            restaurant = await restaurant_service.get_restaurant_details(restaurant_id)
            if not restaurant:
                print(f"Failed to get restaurant details for order {order.id}")
                return

            # Calculate fare (simple calculation based on subtotal + delivery fee)
            fare = float(order.subtotal + order.delivery_fee)  # type: ignore

            # For now, we'll skip coordinates as they would require geocoding
            # In a real implementation, you'd geocode the addresses
            delivery_address = str(order.delivery_address)  # type: ignore
            customer_notes = str(order.delivery_note) if order.delivery_note else None  # type: ignore

            await driver_service.initiate_driver_assignment(
                order_id=str(order.id),
                pickup_address=restaurant.address,
                pickup_lat=None,  # Would need geocoding
                pickup_lng=None,  # Would need geocoding
                delivery_address=delivery_address,
                delivery_lat=None,  # Would need geocoding
                delivery_lng=None,  # Would need geocoding
                fare=fare,
                customer_notes=customer_notes,
            )
        except Exception as e:
            # Log error but don't fail the status update
            print(f"Failed to initiate driver assignment for order {order.id}: {e}")

    async def _handle_driver_accepted(self, order: models.Order) -> None:
        """Handle driver acceptance - complete the order lifecycle"""
        # Order lifecycle is complete - no further actions needed
        pass

    async def _handle_driver_rejected(self, order: models.Order) -> None:
        """Handle driver rejection - terminate order lifecycle"""
        # No retry logic - order lifecycle is terminated
        pass

    def is_terminal_status(self, status: schemas.OrderStatus) -> bool:
        """Check if a status is terminal (no further transitions allowed)"""
        terminal_statuses = {
            schemas.OrderStatus.RESTAURANT_REJECTED,
            schemas.OrderStatus.DRIVER_ACCEPTED,
            schemas.OrderStatus.DRIVER_REJECTED,
        }
        return status in terminal_statuses

    def validate_status_transition(
        self, current_status: str, new_status: schemas.OrderStatus
    ) -> bool:
        """
        Validate if a status transition is allowed.
        Returns True if transition is valid, False otherwise.
        """
        # Parse current status
        try:
            current = schemas.OrderStatus(current_status)
        except ValueError:
            return False

        # Cannot transition from terminal states
        if self.is_terminal_status(current):
            return False

        # Define valid transitions
        valid_transitions = {
            schemas.OrderStatus.PENDING_RESTAURANT: {
                schemas.OrderStatus.RESTAURANT_ACCEPTED,
                schemas.OrderStatus.RESTAURANT_REJECTED,
            },
            schemas.OrderStatus.RESTAURANT_ACCEPTED: {
                schemas.OrderStatus.DRIVER_ACCEPTED,
                schemas.OrderStatus.DRIVER_REJECTED,
            },
        }

        return new_status in valid_transitions.get(current, set())
