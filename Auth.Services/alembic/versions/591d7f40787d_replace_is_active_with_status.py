"""replace is_active with status

Revision ID: 591d7f40787d
Revises: 7ab0a2057ff7
Create Date: 2026-01-14 15:51:02.867174

"""
from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision: str = '591d7f40787d'
down_revision: Union[str, Sequence[str], None] = '7ab0a2057ff7'
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    """Upgrade schema."""
    pass


def downgrade() -> None:
    """Downgrade schema."""
    pass
