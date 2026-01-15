-- Create databases for united services
CREATE DATABASE order_service_db;
CREATE DATABASE driver_services;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE order_service_db TO postgres;
GRANT ALL PRIVILEGES ON DATABASE driver_services TO postgres;

-- Create auth_services database
CREATE DATABASE auth_services;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE auth_services TO postgres;
