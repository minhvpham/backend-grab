"""add category templates table

Revision ID: add_category_templates
Revises: add_discounted_price
Create Date: 2026-01-17 00:10:00.000000

"""
from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision: str = 'add_category_templates'
down_revision: Union[str, Sequence[str], None] = 'add_discounted_price'
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    """Upgrade schema."""
    # Create category_templates table
    op.create_table('category_templates',
        sa.Column('id', sa.Integer(), nullable=False),
        sa.Column('name', sa.String(length=100), nullable=False),
        sa.Column('created_at', sa.DateTime(), nullable=True),
        sa.PrimaryKeyConstraint('id'),
        sa.UniqueConstraint('name')
    )
    op.create_index(op.f('ix_category_templates_id'), 'category_templates', ['id'], unique=False)
    
    # Insert default category templates
    op.execute("""
        INSERT INTO category_templates (name, created_at) VALUES
        ('Đại hạ giá', NOW()),
        ('Ăn vặt', NOW()),
        ('Ăn trưa', NOW()),
        ('Đồ uống', NOW())
        ON CONFLICT (name) DO NOTHING
    """)


def downgrade() -> None:
    """Downgrade schema."""
    op.drop_index(op.f('ix_category_templates_id'), table_name='category_templates')
    op.drop_table('category_templates')
