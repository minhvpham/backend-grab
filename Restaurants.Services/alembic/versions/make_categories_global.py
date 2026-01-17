"""make categories global

Revision ID: make_categories_global
Revises: add_category_templates
Create Date: 2026-01-17 00:20:00.000000

"""
from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa


# revision identifiers, used by Alembic.
revision: str = 'make_categories_global'
down_revision: Union[str, Sequence[str], None] = 'add_category_templates'
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    """Upgrade schema."""
    # Drop category_templates table (no longer needed)
    op.drop_index('ix_category_templates_id', table_name='category_templates')
    op.drop_table('category_templates')
    
    # Drop foreign key constraint from menu_items to categories
    op.drop_constraint('menu_items_category_id_fkey', 'menu_items', type_='foreignkey')
    
    # Drop existing categories table
    op.drop_table('categories')
    
    # Recreate categories table as global (without restaurant_id)
    op.create_table('categories',
        sa.Column('id', sa.Integer(), nullable=False),
        sa.Column('name', sa.String(length=100), nullable=False),
        sa.Column('created_at', sa.DateTime(), nullable=True),
        sa.PrimaryKeyConstraint('id'),
        sa.UniqueConstraint('name')
    )
    op.create_index(op.f('ix_categories_id'), 'categories', ['id'], unique=False)
    
    # Insert default categories
    op.execute("""
        INSERT INTO categories (name, created_at) VALUES
        ('Đại hạ giá', NOW()),
        ('Ăn vặt', NOW()),
        ('Ăn trưa', NOW()),
        ('Đồ uống', NOW())
        ON CONFLICT (name) DO NOTHING
    """)
    
    # Recreate foreign key constraint from menu_items to categories
    op.create_foreign_key('menu_items_category_id_fkey', 'menu_items', 'categories', ['category_id'], ['id'])


def downgrade() -> None:
    """Downgrade schema."""
    # Recreate category_templates table
    op.create_table('category_templates',
        sa.Column('id', sa.Integer(), nullable=False),
        sa.Column('name', sa.String(length=100), nullable=False),
        sa.Column('created_at', sa.DateTime(), nullable=True),
        sa.PrimaryKeyConstraint('id'),
        sa.UniqueConstraint('name')
    )
    op.create_index('ix_category_templates_id', 'category_templates', ['id'], unique=False)
    
    # Drop global categories table
    op.drop_index(op.f('ix_categories_id'), table_name='categories')
    op.drop_table('categories')
    
    # Recreate categories with restaurant_id
    op.create_table('categories',
        sa.Column('id', sa.Integer(), nullable=False),
        sa.Column('restaurant_id', sa.Integer(), nullable=False),
        sa.Column('name', sa.String(length=100), nullable=False),
        sa.ForeignKeyConstraint(['restaurant_id'], ['restaurants.id'], ),
        sa.PrimaryKeyConstraint('id')
    )
    op.create_index(op.f('ix_categories_id'), 'categories', ['id'], unique=False)
