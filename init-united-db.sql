-- Create databases for united services
CREATE DATABASE order_service_db;
CREATE DATABASE driver_services;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE order_service_db TO postgres;
GRANT ALL PRIVILEGES ON DATABASE driver_services TO postgres;
