# Order Service API

Microservice quản lý **Đơn hàng** và **Hồ Sơ Người Dùng (Profile)** cho hệ thống Giao Hàng Thực Phẩm (Food Delivery).

## 🛠️ Tech Stack

| Công nghệ | Version | Mô tả |
|-----------|---------|-------|
| **FastAPI** | 0.109.0 | Web Framework |
| **SQLAlchemy** | 2.0.25 | ORM |
| **PostgreSQL** | 16 | Database |
| **Docker** | - | Container |
| **Pydantic** | 2.5.3 | Data Validation |

## 🚀 Quick Start

```bash
# Chạy với Docker
docker-compose up -d

# API: http://localhost:8002
# Swagger UI: http://localhost:8002/docs
```

## 📊 Database Schema

### Profiles Table (Thông tin cá nhân)
| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary Key |
| name | VARCHAR(255) | Tên người dùng |
| email | VARCHAR(255) | Email (unique) |
| phone | VARCHAR(20) | Số điện thoại (unique) |
| password | VARCHAR(255) | Mật khẩu (SHA256 hash) |
| role | VARCHAR(20) | user / seller / shipper / admin |
| avatar | VARCHAR(500) | URL avatar |
| address | TEXT | Địa chỉ |

### Orders Table
| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary Key |
| profile_id | UUID | FK to profiles |
| restaurant_id | UUID | FK to Restaurant Service |
| driver_id | UUID | FK to Driver Service |
| status | VARCHAR(30) | Trạng thái đơn |
| payment_status | VARCHAR(20) | unpaid / paid / refunded |
| total_amount | NUMERIC(12,2) | Tổng tiền |

### Order Items Table
| Column | Type | Description |
|--------|------|-------------|
| id | UUID | Primary Key |
| order_id | UUID | FK to orders |
| product_name | VARCHAR(255) | Tên sản phẩm |
| quantity | INTEGER | Số lượng |
| unit_price | NUMERIC(12,2) | Đơn giá |

## 📡 API Endpoints

### Profiles API (5 endpoints)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/v1/profiles` | Tạo profile mới |
| GET | `/api/v1/profiles` | Lấy danh sách profiles |
| GET | `/api/v1/profiles/{id}` | Lấy chi tiết profile |
| PUT | `/api/v1/profiles/{id}` | Cập nhật profile |
| DELETE | `/api/v1/profiles/{id}` | Xóa profile |

### Orders API (10 endpoints)

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/v1/orders` | Tạo đơn hàng mới |
| GET | `/api/v1/orders` | Lấy danh sách đơn hàng |
| GET | `/api/v1/orders/{id}` | Lấy chi tiết đơn hàng |
| PUT | `/api/v1/orders/{id}` | Cập nhật đơn hàng |
| DELETE | `/api/v1/orders/{id}` | Xóa đơn hàng |
| POST | `/api/v1/orders/{id}/cancel` | Hủy đơn hàng |
| GET | `/api/v1/orders/profile/{profile_id}` | Lấy đơn hàng theo profile |
| GET | `/api/v1/orders/driver/{driver_id}` | Lấy đơn hàng theo driver |
| GET | `/api/v1/orders/restaurant/{id}` | Lấy đơn hàng theo nhà hàng |
| POST | `/api/v1/orders/{id}/assign-driver` | Gán driver cho đơn hàng |

## 🔄 Order Status Flow

```
pending → confirmed → preparing → ready → finding_driver → delivering → delivered
                                                                    ↘ cancelled
```

## 📁 Project Structure

```
order-service/
├── app/
│   ├── main.py          # FastAPI entry point
│   ├── models.py        # SQLAlchemy models
│   ├── schemas.py       # Pydantic schemas
│   ├── crud.py          # Database operations
│   ├── database.py      # DB connection
│   └── routers/
│       ├── orders.py    # Order endpoints
│       └── profiles.py  # Profile endpoints
├── dump/db/
│   └── init.sql         # Database initialization
├── docker-compose.yml
├── Dockerfile
├── requirements.txt
└── README.md
```

## 🔌 Ports

| Service | Port |
|---------|------|
| Order Service API | 8002 |
| PostgreSQL | 5434 |

## 🧪 Sample Data

Database được khởi tạo với:
- 6 profiles mẫu (3 user, 2 shipper, 1 admin)
- 5 orders mẫu với các trạng thái khác nhau
- 11 order items mẫu

## 👥 Team

| Thành viên | Service |
|------------|---------|
| Mphuc310771 | Order Service |
| minhvpham | Restaurant Service |
| Duyyy123 | Driver Service |
| HCMUS-HQHuy | Auth Service |

## 📄 License

MIT License
