-- Database initialization script for Order Service (PostgreSQL)
-- This file will be executed when PostgreSQL container starts

-- Create orders table
CREATE TABLE IF NOT EXISTS orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    restaurant_id UUID NOT NULL,
    driver_id UUID,
    status VARCHAR(30) DEFAULT 'pending' NOT NULL,
    payment_status VARCHAR(20) DEFAULT 'unpaid' NOT NULL,
    payment_method VARCHAR(50),
    delivery_address TEXT NOT NULL,
    delivery_note TEXT,
    subtotal NUMERIC(12, 2) DEFAULT 0,
    delivery_fee NUMERIC(12, 2) DEFAULT 0,
    discount NUMERIC(12, 2) DEFAULT 0,
    total_amount NUMERIC(12, 2) DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create order_items table
CREATE TABLE IF NOT EXISTS order_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    product_id UUID NOT NULL,
    product_name VARCHAR(255) NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 1,
    unit_price NUMERIC(12, 2) NOT NULL,
    note TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes
CREATE INDEX IF NOT EXISTS idx_orders_user_id ON orders(user_id);
CREATE INDEX IF NOT EXISTS idx_orders_restaurant_id ON orders(restaurant_id);
CREATE INDEX IF NOT EXISTS idx_orders_driver_id ON orders(driver_id);
CREATE INDEX IF NOT EXISTS idx_orders_status ON orders(status);
CREATE INDEX IF NOT EXISTS idx_order_items_order_id ON order_items(order_id);

-- =============================================
-- DATABASE DUMPS - Sample Test Data
-- =============================================

-- Insert sample orders
INSERT INTO orders (id, user_id, restaurant_id, driver_id, status, payment_status, payment_method, delivery_address, subtotal, delivery_fee, total_amount) VALUES
('660e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440001', '770e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440004', 'delivered', 'paid', 'cash', '123 Nguyễn Huệ, Quận 1, TP.HCM', 150000, 15000, 165000),
('660e8400-e29b-41d4-a716-446655440002', '550e8400-e29b-41d4-a716-446655440001', '770e8400-e29b-41d4-a716-446655440001', NULL, 'pending', 'unpaid', 'momo', '123 Nguyễn Huệ, Quận 1, TP.HCM', 200000, 15000, 215000),
('660e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440002', '770e8400-e29b-41d4-a716-446655440002', NULL, 'preparing', 'paid', 'card', '456 Lê Lợi, Quận 3, TP.HCM', 85000, 15000, 100000),
('660e8400-e29b-41d4-a716-446655440004', '550e8400-e29b-41d4-a716-446655440001', '770e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440005', 'delivering', 'paid', 'zalopay', '789 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM', 120000, 20000, 140000),
('660e8400-e29b-41d4-a716-446655440005', '550e8400-e29b-41d4-a716-446655440003', '770e8400-e29b-41d4-a716-446655440001', NULL, 'confirmed', 'paid', 'momo', '321 Võ Văn Tần, Quận 3, TP.HCM', 95000, 15000, 110000)
ON CONFLICT (id) DO NOTHING;

-- Insert sample order items
INSERT INTO order_items (id, order_id, product_id, product_name, quantity, unit_price) VALUES
-- Order 1 items
('880e8400-e29b-41d4-a716-446655440001', '660e8400-e29b-41d4-a716-446655440001', '990e8400-e29b-41d4-a716-446655440001', 'Phở Bò Đặc Biệt', 2, 50000),
('880e8400-e29b-41d4-a716-446655440002', '660e8400-e29b-41d4-a716-446655440001', '990e8400-e29b-41d4-a716-446655440002', 'Nước Chanh', 2, 25000),
-- Order 2 items
('880e8400-e29b-41d4-a716-446655440003', '660e8400-e29b-41d4-a716-446655440002', '990e8400-e29b-41d4-a716-446655440003', 'Bún Chả Hà Nội', 2, 60000),
('880e8400-e29b-41d4-a716-446655440004', '660e8400-e29b-41d4-a716-446655440002', '990e8400-e29b-41d4-a716-446655440004', 'Trà Đá', 4, 5000),
('880e8400-e29b-41d4-a716-446655440005', '660e8400-e29b-41d4-a716-446655440002', '990e8400-e29b-41d4-a716-446655440005', 'Nem Rán', 2, 30000),
-- Order 3 items
('880e8400-e29b-41d4-a716-446655440006', '660e8400-e29b-41d4-a716-446655440003', '990e8400-e29b-41d4-a716-446655440006', 'Cơm Sườn Bì Chả', 1, 55000),
('880e8400-e29b-41d4-a716-446655440007', '660e8400-e29b-41d4-a716-446655440003', '990e8400-e29b-41d4-a716-446655440007', 'Nước Mía', 1, 30000),
-- Order 4 items
('880e8400-e29b-41d4-a716-446655440008', '660e8400-e29b-41d4-a716-446655440004', '990e8400-e29b-41d4-a716-446655440008', 'Bánh Mì Thịt Nướng', 3, 30000),
('880e8400-e29b-41d4-a716-446655440009', '660e8400-e29b-41d4-a716-446655440004', '990e8400-e29b-41d4-a716-446655440009', 'Trà Sữa Trân Châu', 2, 35000),
-- Order 5 items
('880e8400-e29b-41d4-a716-446655440010', '660e8400-e29b-41d4-a716-446655440005', '990e8400-e29b-41d4-a716-446655440010', 'Gỏi Cuốn', 4, 15000),
('880e8400-e29b-41d4-a716-446655440011', '660e8400-e29b-41d4-a716-446655440005', '990e8400-e29b-41d4-a716-446655440011', 'Nước Ép Cam', 2, 25000)
ON CONFLICT (id) DO NOTHING;
