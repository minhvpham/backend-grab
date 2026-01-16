"""replace is_active with status

Revision ID: 0227e3b20ed0
Revises: ba1d57173c82
Create Date: 2026-01-14 16:20:38.329616

"""
from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision: str = '0227e3b20ed0'
down_revision: Union[str, Sequence[str], None] = 'ba1d57173c82'
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    """Upgrade schema."""
    # 1. Tạo ENUM trước
    user_status_enum = sa.Enum(
        'active',
        'inactive',
        'pending',
        'banned',
        name='user_status_enum'
    )
    user_status_enum.create(op.get_bind(), checkfirst=True)

    # 2. Thêm cột status
    op.add_column(
        'users',
        sa.Column(
            'status',
            user_status_enum,
            nullable=False,
            server_default='active'
        )
    )
    op.drop_column('users', 'is_active')
    op.alter_column('users', 'status', server_default=None)
    # ### end Alembic commands ###


def downgrade() -> None:
    op.add_column(
        'users',
        sa.Column(
            'is_active', sa.Boolean(),
            nullable=False,
            server_default=sa.true()
        )
    )

    # 2. Map ngược
    op.execute("""
        UPDATE users
        SET is_active = CASE
            WHEN status = 'active' THEN true
            ELSE false
        END
    """)

    # 3. Xóa status
    op.drop_column('users', 'status')

    # 4. Xóa ENUM
    sa.Enum(name='user_status_enum').drop(op.get_bind(), checkfirst=True)
