"""add discounted_price to menu_items

Revision ID: add_discounted_price
Revises: remove_users_fk
Create Date: 2026-01-17 00:00:00.000000

"""
from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision: str = 'add_discounted_price'
down_revision: Union[str, Sequence[str], None] = 'remove_users_fk'
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    """Upgrade schema."""
    op.add_column('menu_items', sa.Column('discounted_price', sa.Numeric(precision=15, scale=2), nullable=True))


def downgrade() -> None:
    """Downgrade schema."""
    op.drop_column('menu_items', 'discounted_price')
