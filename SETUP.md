# 🚀 HƯỚNG DẪN SETUP ORDER SERVICE

## 📋 Yêu cầu
- Docker Desktop (đã bật)
- Git

---

## ⚡ Quick Start

### 1. Clone và chạy Docker
```bash
cd order-service
docker-compose up -d
```

### 2. Truy cập API
- **Swagger UI**: http://localhost:8002/docs
- **ReDoc**: http://localhost:8002/redoc
- **Health check**: http://localhost:8002/health

---

## 📖 API Endpoints

### Orders CRUD

| Method | Endpoint | Mô tả | Request Body |
|--------|----------|-------|--------------|
| `GET` | `/api/v1/orders` | Lấy tất cả đơn hàng | - |
| `POST` | `/api/v1/orders` | Tạo đơn hàng mới | `{user_id, restaurant_id, delivery_address, items[]}` |
| `GET` | `/api/v1/orders/{id}` | Lấy chi tiết 1 đơn | - |
| `PUT` | `/api/v1/orders/{id}` | Cập nhật đơn hàng | `{status?, driver_id?, ...}` |
| `DELETE` | `/api/v1/orders/{id}` | Xóa đơn hàng | - |
| `POST` | `/api/v1/orders/{id}/cancel` | Hủy đơn hàng | - |

### Lấy đơn theo User/Driver/Restaurant

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `GET` | `/api/v1/orders/user/{user_id}` | Đơn hàng của user |
| `GET` | `/api/v1/orders/driver/{driver_id}` | Đơn hàng của driver |
| `GET` | `/api/v1/orders/restaurant/{restaurant_id}` | Đơn hàng của nhà hàng |
| `POST` | `/api/v1/orders/{id}/assign-driver?driver_id=xxx` | Gán driver cho đơn |

---

## 📝 Ví dụ Request

### Tạo đơn hàng mới
```json
POST /api/v1/orders
{
  "user_id": "550e8400-e29b-41d4-a716-446655440001",
  "restaurant_id": "770e8400-e29b-41d4-a716-446655440001",
  "delivery_address": "123 Nguyễn Huệ, Q1, TP.HCM",
  "delivery_note": "Gọi trước khi giao",
  "payment_method": "momo",
  "items": [
    {
      "product_id": "990e8400-e29b-41d4-a716-446655440001",
      "product_name": "Phở Bò",
      "quantity": 2,
      "unit_price": 50000
    }
  ]
}
```

### Cập nhật trạng thái đơn
```json
PUT /api/v1/orders/{order_id}
{
  "status": "confirmed"
}
```

### Response mẫu
```json
{
  "success": true,
  "message": "Tạo đơn hàng thành công",
  "data": {
    "id": "660e8400-...",
    "user_id": "550e8400-...",
    "status": "pending",
    "total_amount": 115000,
    "items": [...]
  }
}
```

---

## 🗄️ Database

### Port: `5434`
### Connection String:
```
postgresql://postgres:1@localhost:5434/order_service_db
```

### Dump database (sau khi thay đổi):
```bash
./dump-db-script.sh
```

---

## 🔧 Trạng thái đơn hàng (Order Status)

| Status | Mô tả |
|--------|-------|
| `pending` | Chờ xác nhận |
| `confirmed` | Đã xác nhận |
| `preparing` | Đang chuẩn bị |
| `ready` | Sẵn sàng giao |
| `finding_driver` | Đang tìm driver |
| `delivering` | Đang giao |
| `delivered` | Đã giao |
| `cancelled` | Đã hủy |

---

## 🔗 Ports Summary

| Service | Port |
|---------|------|
| User Service API | 8001 |
| **Order Service API** | **8002** |
| Driver Service API | TBD |
| User Service DB | 5433 |
| **Order Service DB** | **5434** |
| Driver Service DB | 5432 |

---

## ❓ Troubleshooting

### Docker không chạy được
1. Mở Docker Desktop
2. Đợi Docker khởi động xong
3. Chạy lại `docker-compose up -d`

### Port đã được sử dụng
```bash
# Kiểm tra port
netstat -ano | findstr "8002"

# Đổi port trong docker-compose.yml nếu cần
```

### Xem logs
```bash
docker-compose logs -f order-service
```
