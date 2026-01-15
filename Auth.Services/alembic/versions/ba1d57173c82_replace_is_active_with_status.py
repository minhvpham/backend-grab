"""replace is_active with status

Revision ID: ba1d57173c82
Revises: 591d7f40787d
Create Date: 2026-01-14 16:17:50.941796

"""
from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision: str = 'ba1d57173c82'
down_revision: Union[str, Sequence[str], None] = '591d7f40787d'
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    """Upgrade schema."""
    pass


def downgrade() -> None:
    """Downgrade schema."""
    pass
