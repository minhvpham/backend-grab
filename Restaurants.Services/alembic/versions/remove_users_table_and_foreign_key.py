"""remove users table and foreign key

Revision ID: remove_users_fk
Revises: 782833f46426
Create Date: 2026-01-16 00:00:00.000000

"""
from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision: str = 'remove_users_fk'
down_revision: Union[str, Sequence[str], None] = '0f5ac9437597'
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    """Upgrade schema."""
    # Drop foreign key constraint from restaurants table
    op.drop_constraint('restaurants_owner_id_fkey', 'restaurants', type_='foreignkey')
    
    # Drop users table
    op.drop_index('ix_users_id', table_name='users')
    op.drop_index('ix_users_email', table_name='users')
    op.drop_table('users')
    
    # Drop the RoleEnum type if it exists
    op.execute("DROP TYPE IF EXISTS roleenum")


def downgrade() -> None:
    """Downgrade schema."""
    # Recreate RoleEnum type
    op.execute("CREATE TYPE roleenum AS ENUM ('user', 'seller', 'shipper', 'admin')")
    
    # Recreate users table
    op.create_table('users',
        sa.Column('id', sa.Integer(), nullable=False),
        sa.Column('email', sa.String(), nullable=False),
        sa.Column('hashed_password', sa.String(), nullable=False),
        sa.Column('role', sa.Enum('user', 'seller', 'shipper', 'admin', name='roleenum'), nullable=True),
        sa.Column('is_active', sa.Boolean(), nullable=True),
        sa.Column('is_deleted', sa.Boolean(), nullable=True),
        sa.Column('created_at', sa.DateTime(timezone=True), server_default=sa.text('now()'), nullable=True),
        sa.Column('updated_at', sa.DateTime(timezone=True), server_default=sa.text('now()'), nullable=True),
        sa.PrimaryKeyConstraint('id')
    )
    op.create_index('ix_users_email', 'users', ['email'], unique=True)
    op.create_index('ix_users_id', 'users', ['id'], unique=False)
    
    # Recreate foreign key constraint
    op.create_foreign_key('restaurants_owner_id_fkey', 'restaurants', 'users', ['owner_id'], ['id'])
