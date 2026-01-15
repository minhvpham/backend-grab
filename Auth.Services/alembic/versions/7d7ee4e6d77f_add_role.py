"""add role

Revision ID: 7d7ee4e6d77f
Revises: 54817d60d36c
Create Date: 2026-01-11 14:10:19.729979
"""
from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa
from sqlalchemy.dialects import postgresql

# revision identifiers, used by Alembic.
revision: str = '7d7ee4e6d77f'
down_revision: Union[str, Sequence[str], None] = '54817d60d36c'
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None

# Tạo enum type PostgreSQL
role_enum = postgresql.ENUM('user', 'seller', 'shipper', 'admin', name='roleenum', create_type=True)

def upgrade() -> None:
    """Upgrade schema."""
    # 1️⃣ Tạo enum type trước
    role_enum.create(op.get_bind(), checkfirst=True)

    # 2️⃣ Thêm cột role và is_active
    op.add_column('users', sa.Column('role', role_enum, nullable=True))
    op.add_column('users', sa.Column('is_active', sa.Boolean(), nullable=True))

def downgrade() -> None:
    """Downgrade schema."""
    # 1️⃣ Xóa cột trước
    op.drop_column('users', 'is_active')
    op.drop_column('users', 'role')

    # 2️⃣ Xóa enum type
    role_enum.drop(op.get_bind(), checkfirst=True)
