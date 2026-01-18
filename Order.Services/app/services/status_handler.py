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
            await driver_service.initiate_driver_assignment(str(order.id))
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
