# Order Service API

Microservice quản lý đơn hàng cho hệ thống Giao Hàng Thực Phẩm.

## Tech Stack
- **Framework**: FastAPI (Python 3.11)
- **Database**: PostgreSQL 16
- **Container**: Docker

## Quick Start

```bash
# Chạy với Docker
docker-compose up -d

# API sẽ chạy tại: http://localhost:8002
# Swagger docs: http://localhost:8002/docs
```

## API Endpoints

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/v1/orders` | Lấy danh sách đơn hàng |
| POST | `/api/v1/orders` | Tạo đơn hàng mới |
| GET | `/api/v1/orders/{id}` | Lấy chi tiết đơn hàng |
| PUT | `/api/v1/orders/{id}` | Cập nhật đơn hàng |
| DELETE | `/api/v1/orders/{id}` | Xóa đơn hàng |
| POST | `/api/v1/orders/{id}/cancel` | Hủy đơn hàng |
| GET | `/api/v1/orders/user/{user_id}` | Lấy đơn hàng theo user |
| GET | `/api/v1/orders/driver/{driver_id}` | Lấy đơn hàng theo driver |
| GET | `/api/v1/orders/restaurant/{restaurant_id}` | Lấy đơn hàng theo nhà hàng |
| POST | `/api/v1/orders/{id}/assign-driver` | Gán driver cho đơn hàng |

## Database

Port: `5434` (tránh conflict với User Service `5433`)

```bash
# Dump database
./dump-db-script.sh
```

## Ports

| Service | Port |
|---------|------|
| Order Service API | 8002 |
| PostgreSQL | 5434 |
