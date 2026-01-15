-- Restaurants Services Database Schema
-- Generated from SQLAlchemy models

-- Create ENUM types first
CREATE TYPE roleenum AS ENUM ('user', 'seller', 'shipper', 'admin');
CREATE TYPE restaurant_status AS ENUM ('PENDING', 'ACTIVE', 'BANNED', 'REJECTED');

-- Create users table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    email VARCHAR UNIQUE NOT NULL,
    hashed_password VARCHAR NOT NULL,
    role roleenum DEFAULT 'user',
    is_active BOOLEAN DEFAULT TRUE,
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create indexes for users
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_id ON users(id);
CREATE INDEX idx_users_role ON users(role);

-- Create restaurants table
CREATE TABLE restaurants (
    id SERIAL PRIMARY KEY,
    owner_id INTEGER NOT NULL REFERENCES users(id),
    name VARCHAR(100) NOT NULL,
    description TEXT,
    address TEXT NOT NULL,
    phone VARCHAR(20),
    opening_hours VARCHAR(100),
    is_open BOOLEAN DEFAULT TRUE,
    rating NUMERIC(2, 1) DEFAULT 0.0,
    business_license_image VARCHAR(255),
    food_safety_certificate_image VARCHAR(255),
    status restaurant_status DEFAULT 'PENDING',
    created_at TIMESTAMP DEFAULT NOW()
);

-- Create indexes for restaurants
CREATE INDEX idx_restaurants_owner_id ON restaurants(owner_id);
CREATE INDEX idx_restaurants_id ON restaurants(id);
CREATE INDEX idx_restaurants_status ON restaurants(status);

-- Create categories table
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    restaurant_id INTEGER NOT NULL REFERENCES restaurants(id),
    name VARCHAR(100) NOT NULL
);

-- Create indexes for categories
CREATE INDEX idx_categories_restaurant_id ON categories(restaurant_id);
CREATE INDEX idx_categories_id ON categories(id);

-- Create menu_items table
CREATE TABLE menu_items (
    id SERIAL PRIMARY KEY,
    restaurant_id INTEGER NOT NULL REFERENCES restaurants(id),
    category_id INTEGER REFERENCES categories(id),
    name VARCHAR(100) NOT NULL,
    description TEXT,
    price NUMERIC(15, 2) NOT NULL,
    image_url VARCHAR(255),
    is_available BOOLEAN DEFAULT TRUE,
    stock_quantity INTEGER,
    created_at TIMESTAMP DEFAULT NOW()
);

-- Create indexes for menu_items
CREATE INDEX idx_menu_items_restaurant_id ON menu_items(restaurant_id);
CREATE INDEX idx_menu_items_category_id ON menu_items(category_id);
CREATE INDEX idx_menu_items_id ON menu_items(id);

-- Optional: Insert sample data
-- Insert sample users
INSERT INTO users (email, hashed_password, role, is_active) VALUES
('restaurant_owner@example.com', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIeWU7u3pK', 'seller', true),
('admin@grab.com', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIeWU7u3pK', 'admin', true)
ON CONFLICT (email) DO NOTHING;

-- Insert sample restaurant
INSERT INTO restaurants (owner_id, name, description, address, phone, opening_hours, status) VALUES
(1, 'Pho 24', 'Traditional Vietnamese restaurant serving authentic pho', '123 Nguyen Trai, District 1, HCMC', '+84-28-1234-5678', '07:00-22:00', 'ACTIVE')
ON CONFLICT DO NOTHING;

-- Insert sample categories
INSERT INTO categories (restaurant_id, name) VALUES
(1, 'Main Dishes'),
(1, 'Beverages'),
(1, 'Desserts')
ON CONFLICT DO NOTHING;

-- Insert sample menu items
INSERT INTO menu_items (restaurant_id, category_id, name, description, price, is_available) VALUES
(1, 1, 'Pho Bo', 'Beef noodle soup with fresh herbs', 45000, true),
(1, 1, 'Bun Cha', 'Grilled pork with rice noodles', 55000, true),
(1, 2, 'Tra Da', 'Iced tea', 15000, true),
(1, 2, 'Ca Phe Sua Da', 'Vietnamese iced coffee', 25000, true),
(1, 3, 'Che Ba Mau', 'Three-color dessert', 30000, true)
ON CONFLICT DO NOTHING;
