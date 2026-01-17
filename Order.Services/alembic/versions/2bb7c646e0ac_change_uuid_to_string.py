"""Change UUID columns to String

Revision ID: 2bb7c646e0ac
Revises: 1aa6c646e0ab
Create Date: 2026-01-17 16:45:00.000000

"""
from alembic import op
import sqlalchemy as sa
from sqlalchemy.dialects import postgresql

# revision identifiers, used by Alembic.
revision = '2bb7c646e0ac'
down_revision = '1aa6c646e0ab'
branch_labels = None
depends_on = None


def upgrade() -> None:
    # Alter orders table
    op.alter_column('orders', 'restaurant_id',
               existing_type=postgresql.UUID(),
               type_=sa.String(length=255),
               existing_nullable=False,
               postgresql_using='restaurant_id::text')
    
    op.alter_column('orders', 'driver_id',
               existing_type=postgresql.UUID(),
               type_=sa.String(length=255),
               existing_nullable=True,
               postgresql_using='driver_id::text')

    # Alter order_items table
    op.alter_column('order_items', 'product_id',
               existing_type=postgresql.UUID(),
               type_=sa.String(length=255),
               existing_nullable=False,
               postgresql_using='product_id::text')


def downgrade() -> None:
    # Revert orders table
    op.alter_column('orders', 'driver_id',
               existing_type=sa.String(length=255),
               type_=postgresql.UUID(),
               existing_nullable=True,
               postgresql_using='driver_id::uuid')
    
    op.alter_column('orders', 'restaurant_id',
               existing_type=sa.String(length=255),
               type_=postgresql.UUID(),
               existing_nullable=False,
               postgresql_using='restaurant_id::uuid')

    # Revert order_items table
    op.alter_column('order_items', 'product_id',
               existing_type=sa.String(length=255),
               type_=postgresql.UUID(),
               existing_nullable=False,
               postgresql_using='product_id::uuid')
