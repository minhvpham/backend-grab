-- Auth Services Database Schema
-- Generated from SQLAlchemy model

-- Create ENUM types first
CREATE TYPE roleenum AS ENUM ('user', 'seller', 'shipper', 'admin');
CREATE TYPE user_status_enum AS ENUM ('active', 'inactive', 'pending', 'banned');

-- Create users table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    email VARCHAR UNIQUE NOT NULL,
    hashed_password VARCHAR NOT NULL,
    role roleenum DEFAULT 'user',
    status user_status_enum DEFAULT 'active' NOT NULL,
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Create indexes
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_id ON users(id);
CREATE INDEX idx_users_role ON users(role);
CREATE INDEX idx_users_status ON users(status);

-- Optional: Insert sample admin user
-- Password hash for "admin123" (bcrypt hash)
INSERT INTO users (email, hashed_password, role, status) VALUES
('admin@grab.com', '$2b$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIeWU7u3pK', 'admin', 'active')
ON CONFLICT (email) DO NOTHING;
