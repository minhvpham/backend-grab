c order_service_db
-- Database initialization script for Order Service (PostgreSQL)
-- This file will be executed when PostgreSQL container starts

-- =============================================
-- USERS TABLE
-- =============================================
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    phone VARCHAR(20) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    role VARCHAR(20) DEFAULT 'user' NOT NULL,
    avatar VARCHAR(500),
    address TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_users_phone ON users(phone);

-- =============================================
-- ORDERS TABLE
-- =============================================
CREATE TABLE IF NOT EXISTS orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id),
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

CREATE INDEX IF NOT EXISTS idx_orders_user_id ON orders(user_id);
CREATE INDEX IF NOT EXISTS idx_orders_restaurant_id ON orders(restaurant_id);
CREATE INDEX IF NOT EXISTS idx_orders_driver_id ON orders(driver_id);
CREATE INDEX IF NOT EXISTS idx_orders_status ON orders(status);

-- =============================================
-- ORDER ITEMS TABLE
-- =============================================
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

CREATE INDEX IF NOT EXISTS idx_order_items_order_id ON order_items(order_id);

-- =============================================
-- SAMPLE DATA - USERS
-- =============================================
INSERT INTO users (id, name, email, phone, password, role, address) VALUES
('550e8400-e29b-41d4-a716-446655440001', 'Nguyễn Văn A', 'nguyenvana@gmail.com', '0901234567', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'user', '123 Nguyễn Huệ, Quận 1, TP.HCM'),
('550e8400-e29b-41d4-a716-446655440002', 'Trần Thị B', 'tranthib@gmail.com', '0912345678', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'user', '456 Lê Lợi, Quận 3, TP.HCM'),
('550e8400-e29b-41d4-a716-446655440003', 'Lê Văn C', 'levanc@gmail.com', '0923456789', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'user', '789 Trần Hưng Đạo, Quận 5, TP.HCM'),
('550e8400-e29b-41d4-a716-446655440004', 'Phạm Văn D', 'driver1@gmail.com', '0934567890', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'shipper', '321 Võ Văn Tần, Quận 3, TP.HCM'),
('550e8400-e29b-41d4-a716-446655440005', 'Hoàng Thị E', 'driver2@gmail.com', '0945678901', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'shipper', '654 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM'),
('550e8400-e29b-41d4-a716-446655440006', 'Admin System', 'admin@grabfood.vn', '0900000000', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'admin', 'HCM')
ON CONFLICT (id) DO NOTHING;

-- =============================================
-- SAMPLE DATA - ORDERS
-- =============================================
INSERT INTO orders (id, user_id, restaurant_id, driver_id, status, payment_status, payment_method, delivery_address, subtotal, delivery_fee, total_amount) VALUES
('660e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440001', '770e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440004', 'delivered', 'paid', 'cash', '123 Nguyễn Huệ, Quận 1, TP.HCM', 150000, 15000, 165000),
('660e8400-e29b-41d4-a716-446655440002', '550e8400-e29b-41d4-a716-446655440001', '770e8400-e29b-41d4-a716-446655440001', NULL, 'pending', 'unpaid', 'momo', '123 Nguyễn Huệ, Quận 1, TP.HCM', 200000, 15000, 215000),
('660e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440002', '770e8400-e29b-41d4-a716-446655440002', NULL, 'preparing', 'paid', 'card', '456 Lê Lợi, Quận 3, TP.HCM', 85000, 15000, 100000),
('660e8400-e29b-41d4-a716-446655440004', '550e8400-e29b-41d4-a716-446655440001', '770e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440005', 'delivering', 'paid', 'zalopay', '789 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM', 120000, 20000, 140000),
('660e8400-e29b-41d4-a716-446655440005', '550e8400-e29b-41d4-a716-446655440003', '770e8400-e29b-41d4-a716-446655440001', NULL, 'confirmed', 'paid', 'momo', '321 Võ Văn Tần, Quận 3, TP.HCM', 95000, 15000, 110000)
ON CONFLICT (id) DO NOTHING;

-- =============================================
-- SAMPLE DATA - ORDER ITEMS
-- =============================================
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
