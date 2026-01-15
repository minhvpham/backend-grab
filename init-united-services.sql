-- Combined database initialization script for both Order and Driver Services
-- This file will be executed when PostgreSQL container starts

-- =============================================
-- ORDER SERVICE TABLES
-- =============================================

-- USERS TABLE
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

-- ORDERS TABLE
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

-- ORDER ITEMS TABLE
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
-- DRIVER SERVICE TABLES
-- =============================================

CREATE TABLE public."DriverLocations" (
    "Id" uuid NOT NULL,
    "DriverId" uuid NOT NULL,
    "Latitude" double precision NOT NULL,
    "Longitude" double precision NOT NULL,
    "Accuracy" double precision,
    "Heading" double precision,
    "Speed" double precision,
    "Timestamp" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "Deleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "UpdateByUserId" text,
    "UpdateByIdentityName" text
);
ALTER TABLE public."DriverLocations" OWNER TO postgres;

CREATE TABLE public."DriverWallets" (
    "Id" uuid NOT NULL,
    "DriverId" uuid NOT NULL,
    "Balance" numeric(18,2) NOT NULL,
    "CashOnHand" numeric(18,2) NOT NULL,
    "TotalEarnings" numeric(18,2) NOT NULL,
    "TotalWithdrawn" numeric(18,2) NOT NULL,
    "LastWithdrawalAt" timestamp with time zone,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "Deleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "UpdateByUserId" text,
    "UpdateByIdentityName" text
);
ALTER TABLE public."DriverWallets" OWNER TO postgres;

CREATE TABLE public."Drivers" (
    "Id" uuid NOT NULL,
    "FullName" character varying(100) NOT NULL,
    "PhoneNumber" character varying(20) NOT NULL,
    "Email" character varying(255) NOT NULL,
    "Status" character varying(50) NOT NULL,
    "VerificationStatus" character varying(50) NOT NULL,
    "VehicleType" character varying(50),
    "LicensePlate" character varying(20),
    "VehicleBrand" character varying(50),
    "VehicleModel" character varying(50),
    "VehicleYear" integer,
    "VehicleColor" character varying(30),
    "LicenseNumber" character varying(50),
    "ProfileImageUrl" character varying(500),
    "VerifiedAt" timestamp with time zone,
    "RejectionReason" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "Deleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "UpdateByUserId" text,
    "UpdateByIdentityName" text
);
ALTER TABLE public."Drivers" OWNER TO postgres;

CREATE TABLE public."Transactions" (
    "Id" uuid NOT NULL,
    "Type" character varying(50) NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "BalanceBefore" numeric(18,2) NOT NULL,
    "BalanceAfter" numeric(18,2) NOT NULL,
    "OrderId" character varying(100),
    "Reference" character varying(200),
    "Description" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    "WalletId" uuid NOT NULL
);
ALTER TABLE public."Transactions" OWNER TO postgres;

CREATE TABLE public."TripHistories" (
    "Id" uuid NOT NULL,
    "DriverId" uuid NOT NULL,
    "OrderId" character varying(100) NOT NULL,
    "Status" character varying(50) NOT NULL,
    "PickupAddress" character varying(500) NOT NULL,
    "PickupLatitude" double precision NOT NULL,
    "PickupLongitude" double precision NOT NULL,
    "DeliveryAddress" character varying(500) NOT NULL,
    "DeliveryLatitude" double precision NOT NULL,
    "DeliveryLongitude" double precision NOT NULL,
    "DistanceKm" double precision,
    "DurationMinutes" integer,
    "Fare" numeric(18,2) NOT NULL,
    "CashCollected" numeric(18,2),
    "AssignedAt" timestamp with time zone NOT NULL,
    "AcceptedAt" timestamp with time zone,
    "PickedUpAt" timestamp with time zone,
    "DeliveredAt" timestamp with time zone,
    "CancelledAt" timestamp with time zone,
    "CancellationReason" character varying(500),
    "FailureReason" character varying(500),
    "CustomerNotes" character varying(1000),
    "DriverNotes" character varying(1000),
    "CustomerRating" integer,
    "CustomerFeedback" character varying(1000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "Deleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "UpdateByUserId" text,
    "UpdateByIdentityName" text
);
ALTER TABLE public."TripHistories" OWNER TO postgres;

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);
ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

-- Constraints and Indexes for Driver Service tables
ALTER TABLE ONLY public."DriverLocations" ADD CONSTRAINT "PK_DriverLocations" PRIMARY KEY ("Id");
ALTER TABLE ONLY public."DriverWallets" ADD CONSTRAINT "PK_DriverWallets" PRIMARY KEY ("Id");
ALTER TABLE ONLY public."Drivers" ADD CONSTRAINT "PK_Drivers" PRIMARY KEY ("Id");
ALTER TABLE ONLY public."Transactions" ADD CONSTRAINT "PK_Transactions" PRIMARY KEY ("Id");
ALTER TABLE ONLY public."TripHistories" ADD CONSTRAINT "PK_TripHistories" PRIMARY KEY ("Id");
ALTER TABLE ONLY public."__EFMigrationsHistory" ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");

CREATE UNIQUE INDEX "IX_DriverLocations_DriverId" ON public."DriverLocations" USING btree ("DriverId");
CREATE INDEX "IX_DriverLocations_Latitude_Longitude" ON public."DriverLocations" USING btree ("Latitude", "Longitude");
CREATE INDEX "IX_DriverLocations_Timestamp" ON public."DriverLocations" USING btree ("Timestamp");
CREATE UNIQUE INDEX "IX_DriverWallets_DriverId" ON public."DriverWallets" USING btree ("DriverId");
CREATE INDEX "IX_DriverWallets_IsActive" ON public."DriverWallets" USING btree ("IsActive");
CREATE UNIQUE INDEX "IX_Drivers_Email" ON public."Drivers" USING btree ("Email");
CREATE INDEX "IX_Drivers_Status" ON public."Drivers" USING btree ("Status");
CREATE INDEX "IX_Drivers_VerificationStatus" ON public."Drivers" USING btree ("VerificationStatus");
CREATE INDEX "IX_Transactions_CreatedAt" ON public."Transactions" USING btree ("CreatedAt");
CREATE INDEX "IX_Transactions_OrderId" ON public."Transactions" USING btree ("OrderId");
CREATE INDEX "IX_Transactions_Type" ON public."Transactions" USING btree ("Type");
CREATE INDEX "IX_Transactions_WalletId" ON public."Transactions" USING btree ("WalletId");
CREATE INDEX "IX_TripHistories_AssignedAt" ON public."TripHistories" USING btree ("AssignedAt");
CREATE INDEX "IX_TripHistories_DeliveredAt" ON public."TripHistories" USING btree ("DeliveredAt");
CREATE INDEX "IX_TripHistories_DriverId" ON public."TripHistories" USING btree ("DriverId");
CREATE UNIQUE INDEX "IX_TripHistories_OrderId" ON public."TripHistories" USING btree ("OrderId");
CREATE INDEX "IX_TripHistories_Status" ON public."TripHistories" USING btree ("Status");

ALTER TABLE ONLY public."Transactions" ADD CONSTRAINT "FK_Transactions_DriverWallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES public."DriverWallets"("Id") ON DELETE CASCADE;

-- =============================================
-- SAMPLE DATA - ORDER SERVICE
-- =============================================
INSERT INTO users (id, name, email, phone, password, role, address) VALUES
('550e8400-e29b-41d4-a716-446655440001', 'Nguyễn Văn A', 'nguyenvana@gmail.com', '0901234567', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'user', '123 Nguyễn Huệ, Quận 1, TP.HCM'),
('550e8400-e29b-41d4-a716-446655440002', 'Trần Thị B', 'tranthib@gmail.com', '0912345678', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'user', '456 Lê Lợi, Quận 3, TP.HCM'),
('550e8400-e29b-41d4-a716-446655440003', 'Lê Văn C', 'levanc@gmail.com', '0923456789', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'user', '789 Trần Hưng Đạo, Quận 5, TP.HCM'),
('550e8400-e29b-41d4-a716-446655440004', 'Phạm Văn D', 'driver1@gmail.com', '0934567890', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'shipper', '321 Võ Văn Tần, Quận 3, TP.HCM'),
('550e8400-e29b-41d4-a716-446655440005', 'Hoàng Thị E', 'driver2@gmail.com', '0945678901', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'shipper', '654 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM'),
('550e8400-e29b-41d4-a716-446655440006', 'Admin System', 'admin@grabfood.vn', '0900000000', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', 'admin', 'HCM')
ON CONFLICT (id) DO NOTHING;

INSERT INTO orders (id, user_id, restaurant_id, driver_id, status, payment_status, payment_method, delivery_address, subtotal, delivery_fee, total_amount) VALUES
('660e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440001', '770e8400-e29b-41d4-a716-446655440001', '550e8400-e29b-41d4-a716-446655440004', 'delivered', 'paid', 'cash', '123 Nguyễn Huệ, Quận 1, TP.HCM', 150000, 15000, 165000),
('660e8400-e29b-41d4-a716-446655440002', '550e8400-e29b-41d4-a716-446655440001', '770e8400-e29b-41d4-a716-446655440001', NULL, 'pending', 'unpaid', 'momo', '123 Nguyễn Huệ, Quận 1, TP.HCM', 200000, 15000, 215000),
('660e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440002', '770e8400-e29b-41d4-a716-446655440002', NULL, 'preparing', 'paid', 'card', '456 Lê Lợi, Quận 3, TP.HCM', 85000, 15000, 100000),
('660e8400-e29b-41d4-a716-446655440004', '550e8400-e29b-41d4-a716-446655440001', '770e8400-e29b-41d4-a716-446655440003', '550e8400-e29b-41d4-a716-446655440005', 'delivering', 'paid', 'zalopay', '789 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM', 120000, 20000, 140000),
('660e8400-e29b-41d4-a716-446655440005', '550e8400-e29b-41d4-a716-446655440003', '770e8400-e29b-41d4-a716-446655440001', NULL, 'confirmed', 'paid', 'momo', '321 Võ Văn Tần, Quận 3, TP.HCM', 95000, 15000, 110000)
ON CONFLICT (id) DO NOTHING;

INSERT INTO order_items (id, order_id, product_id, product_name, quantity, unit_price) VALUES
('880e8400-e29b-41d4-a716-446655440001', '660e8400-e29b-41d4-a716-446655440001', '990e8400-e29b-41d4-a716-446655440001', 'Phở Bò Đặc Biệt', 2, 50000),
('880e8400-e29b-41d4-a716-446655440002', '660e8400-e29b-41d4-a716-446655440001', '990e8400-e29b-41d4-a716-446655440002', 'Nước Chanh', 2, 25000),
('880e8400-e29b-41d4-a716-446655440003', '660e8400-e29b-41d4-a716-446655440002', '990e8400-e29b-41d4-a716-446655440003', 'Bún Chả Hà Nội', 2, 60000),
('880e8400-e29b-41d4-a716-446655440004', '660e8400-e29b-41d4-a716-446655440002', '990e8400-e29b-41d4-a716-446655440004', 'Trà Đá', 4, 5000),
('880e8400-e29b-41d4-a716-446655440005', '660e8400-e29b-41d4-a716-446655440002', '990e8400-e29b-41d4-a716-446655440005', 'Nem Rán', 2, 30000),
('880e8400-e29b-41d4-a716-446655440006', '660e8400-e29b-41d4-a716-446655440003', '990e8400-e29b-41d4-a716-446655440006', 'Cơm Sườn Bì Chả', 1, 55000),
('880e8400-e29b-41d4-a716-446655440007', '660e8400-e29b-41d4-a716-446655440003', '990e8400-e29b-41d4-a716-446655440007', 'Nước Mía', 1, 30000),
('880e8400-e29b-41d4-a716-446655440008', '660e8400-e29b-41d4-a716-446655440004', '990e8400-e29b-41d4-a716-446655440008', 'Bánh Mì Thịt Nướng', 3, 30000),
('880e8400-e29b-41d4-a716-446655440009', '660e8400-e29b-41d4-a716-446655440004', '990e8400-e29b-41d4-a716-446655440009', 'Trà Sữa Trân Châu', 2, 35000),
('880e8400-e29b-41d4-a716-446655440010', '660e8400-e29b-41d4-a716-446655440005', '990e8400-e29b-41d4-a716-446655440010', 'Gỏi Cuốn', 4, 15000),
('880e8400-e29b-41d4-a716-446655440011', '660e8400-e29b-41d4-a716-446655440005', '990e8400-e29b-41d4-a716-446655440011', 'Nước Ép Cam', 2, 25000)
ON CONFLICT (id) DO NOTHING;

-- =============================================
-- SAMPLE DATA - DRIVER SERVICE
-- =============================================
INSERT INTO public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES ('20260112174418_InitialCreate', '8.0.11') ON CONFLICT ("MigrationId") DO NOTHING;

INSERT INTO public."Drivers" ("Id", "FullName", "PhoneNumber", "Email", "Status", "VerificationStatus", "VehicleType", "LicensePlate", "VehicleBrand", "VehicleModel", "VehicleYear", "VehicleColor", "LicenseNumber", "ProfileImageUrl", "VerifiedAt", "RejectionReason", "CreatedAt", "UpdatedAt", "Deleted", "DeletedAt", "UpdateByUserId", "UpdateByIdentityName") VALUES
('0853d007-3ca5-4b21-8a05-371638cd57d3', 'Ngo Hoang Binh', '+84073639689', 'ngo.hoang.binh5703@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '59118594', NULL, '2026-01-13 18:17:35.211837+00', NULL, '2026-01-13 18:17:35.211836+00', '2026-01-13 18:17:35.211837+00', false, NULL, NULL, NULL),
('125c5232-99f0-43e7-bc0b-6efb83b9277a', 'Ngo Thanh Yen', '+84086840602', 'ngo.thanh.yen9817@outlook.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '65353007', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/800.jpg', NULL, NULL, '2026-01-13 18:17:35.211861+00', '2026-01-13 18:17:35.211862+00', false, NULL, NULL, NULL),
('28546a19-406d-4f93-b35d-de57acf1b8b0', 'Tran Hong My', '+84057267664', 'tran.hong.my1274@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '56175973', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/156.jpg', '2026-01-13 18:17:35.211175+00', NULL, '2026-01-13 18:17:35.21044+00', '2026-01-13 18:17:35.211453+00', false, NULL, NULL, NULL),
('2cef4120-ed39-4f25-bf91-888ec7b4b54b', 'Tran Ngoc Thanh', '+84089001824', 'tran.ngoc.thanh7013@gmail.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-01-13 18:17:35.211801+00', NULL, '2026-01-13 18:17:35.211799+00', '2026-01-13 18:17:35.211801+00', false, NULL, NULL, NULL),
('3472a608-0870-462f-a1ba-5efb21cbcace', 'Truong Quoc Minh', '+84086670396', 'truong.quoc.minh3468@gmail.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '44048490', NULL, '2026-01-13 18:17:35.211834+00', NULL, '2026-01-13 18:17:35.211832+00', '2026-01-13 18:17:35.211834+00', false, NULL, NULL, NULL),
('36054f77-0c8b-4bea-b2c2-b4b659a5c94f', 'Vo Ngoc Yen', '+84071331296', 'vo.ngoc.yen3308@outlook.com', 'Offline', 'Rejected', NULL, NULL, NULL, NULL, NULL, NULL, '46361030', NULL, NULL, 'Vehicle does not meet requirements', '2026-01-13 18:17:35.211491+00', '2026-01-13 18:17:35.211641+00', false, NULL, NULL, NULL),
('3f666688-cbaf-4b04-9382-4589e677a1ca', 'Dang Van Phuc', '+84050199797', 'dang.van.phuc8920@email.vn', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '98408824', NULL, NULL, NULL, '2026-01-13 18:17:35.211775+00', '2026-01-13 18:17:35.211775+00', false, NULL, NULL, NULL),
('461f2337-50a5-4e2b-8c9e-69a4b01f48d3', 'Vo Thanh Cuong', '+84079943107', 'vo.thanh.cuong3357@outlook.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '91633468', NULL, NULL, NULL, '2026-01-13 18:17:35.211711+00', '2026-01-13 18:17:35.211711+00', false, NULL, NULL, NULL),
('50fd1821-ad6d-4210-acdf-a14d34744956', 'Vo Minh Anh', '+84051605216', 'vo.minh.anh903@gmail.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-01-13 18:17:35.21176+00', NULL, '2026-01-13 18:17:35.211759+00', '2026-01-13 18:17:35.21176+00', false, NULL, NULL, NULL),
('5540e085-5c96-4a77-ba0d-733ebc7fb7be', 'Hoang Minh My', '+84030889246', 'hoang.minh.my867@outlook.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '29746827', NULL, NULL, NULL, '2026-01-13 18:17:35.211788+00', '2026-01-13 18:17:35.211788+00', false, NULL, NULL, NULL),
('55974ac1-a797-4c14-960d-3dedc8f28324', 'Tran Ngoc Dung', '+84093386784', 'tran.ngoc.dung4363@yahoo.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '45586050', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/242.jpg', NULL, NULL, '2026-01-13 18:17:35.211463+00', '2026-01-13 18:17:35.211474+00', false, NULL, NULL, NULL),
('560882ac-457f-4764-ae7d-4dd2e9cdbae5', 'Trinh Thanh Mai', '+84033534543', 'trinh.thanh.mai7650@gmail.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '22204082', NULL, '2026-01-13 18:17:35.211763+00', NULL, '2026-01-13 18:17:35.211762+00', '2026-01-13 18:17:35.211763+00', false, NULL, NULL, NULL),
('57327cb8-0563-4f6b-a679-7dae9a4ec3d6', 'Ly Duc Anh', '+84083525606', 'ly.duc.anh3224@outlook.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '61814728', NULL, '2026-01-13 18:17:35.211826+00', NULL, '2026-01-13 18:17:35.211825+00', '2026-01-13 18:17:35.211826+00', false, NULL, NULL, NULL),
('64c27678-d3be-4b43-a123-f9aa7415c935', 'Do Xuan Son', '+84094199900', 'do.xuan.son8845@yahoo.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '23820657', NULL, '2026-01-13 18:17:35.211783+00', NULL, '2026-01-13 18:17:35.211781+00', '2026-01-13 18:17:35.211783+00', false, NULL, NULL, NULL),
('76805067-db43-4bc9-9cd6-8e9c06078ab3', 'Ly Quoc Duc', '+84090971585', 'ly.quoc.duc3908@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '42687567', NULL, '2026-01-13 18:17:35.21178+00', NULL, '2026-01-13 18:17:35.211778+00', '2026-01-13 18:17:35.21178+00', false, NULL, NULL, NULL),
('7889b235-f5c7-450f-a009-9692c1e79d4f', 'Luong Minh Tuan', '+84097710685', 'luong.minh.tuan5976@gmail.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '77333806', NULL, '2026-01-13 18:17:35.211692+00', NULL, '2026-01-13 18:17:35.211688+00', '2026-01-13 18:17:35.211692+00', false, NULL, NULL, NULL),
('78b636ed-1bb1-46ee-871c-8243a30adb3f', 'Pham Van Quynh', '+84036980269', 'pham.van.quynh381@outlook.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '70541233', NULL, '2026-01-13 18:17:35.211725+00', NULL, '2026-01-13 18:17:35.211724+00', '2026-01-13 18:17:35.211725+00', false, NULL, NULL, NULL),
('78fbfeb2-3621-48a7-a459-fddc4c85706c', 'Do Quoc Long', '+84092078501', 'do.quoc.long887@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-01-13 18:17:35.211702+00', NULL, '2026-01-13 18:17:35.211701+00', '2026-01-13 18:17:35.211702+00', false, NULL, NULL, NULL),
('7b0416f5-1b2f-4515-a86b-b10961dd5912', 'Ho Quoc Linh', '+84070243400', 'ho.quoc.linh8520@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/155.jpg', '2026-01-13 18:17:35.211803+00', NULL, '2026-01-13 18:17:35.211802+00', '2026-01-13 18:17:35.211803+00', false, NULL, NULL, NULL),
('7ed92b85-bb77-4905-9985-1072e68fa403', 'Vo Quoc Ngoc', '+84056054400', 'vo.quoc.ngoc8956@gmail.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/763.jpg', NULL, NULL, '2026-01-13 18:17:35.211717+00', '2026-01-13 18:17:35.211719+00', false, NULL, NULL, NULL),
('85ce8be8-61bc-469a-923d-2edb790ce2b7', 'Hoang Thanh Dung', '+84087647431', 'hoang.thanh.dung7203@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '51288957', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/4.jpg', '2026-01-13 18:17:35.211794+00', NULL, '2026-01-13 18:17:35.211793+00', '2026-01-13 18:17:35.211794+00', false, NULL, NULL, NULL),
('88602bf8-4770-4da3-aacb-23a6ef587d53', 'Ngo Hong Phong', '+84088582472', 'ngo.hong.phong9928@outlook.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-01-13 18:17:35.21182+00', NULL, '2026-01-13 18:17:35.211818+00', '2026-01-13 18:17:35.21182+00', false, NULL, NULL, NULL),
('8cb0fb98-a8c2-4df2-b4a9-119091d2abd8', 'Truong Thanh Nga', '+84080747712', 'truong.thanh.nga9079@outlook.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-01-13 18:17:35.211747+00', NULL, '2026-01-13 18:17:35.211745+00', '2026-01-13 18:17:35.211747+00', false, NULL, NULL, NULL),
('9359a861-5375-415d-92d9-bcbc550ed67a', 'Duong Xuan Hai', '+84058321236', 'duong.xuan.hai5464@yahoo.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '73852069', NULL, NULL, NULL, '2026-01-13 18:17:35.21174+00', '2026-01-13 18:17:35.21174+00', false, NULL, NULL, NULL),
('93d38328-2151-4d96-ba43-adb846797330', 'Ly Minh Linh', '+84090541700', 'ly.minh.linh5368@yahoo.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/517.jpg', '2026-01-13 18:17:35.211859+00', NULL, '2026-01-13 18:17:35.211857+00', '2026-01-13 18:17:35.211859+00', false, NULL, NULL, NULL),
('93e6dc78-f152-489d-b558-a08c99f37401', 'Trinh Duc Lan', '+84056688549', 'trinh.duc.lan4703@outlook.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '17128509', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/1168.jpg', '2026-01-13 18:17:35.211482+00', NULL, '2026-01-13 18:17:35.21148+00', '2026-01-13 18:17:35.211482+00', false, NULL, NULL, NULL),
('97e51a86-02ec-4198-8baf-d6b7c12d435a', 'Hoang Thanh Huy', '+84052906425', 'hoang.thanh.huy2759@yahoo.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '21527136', NULL, '2026-01-13 18:17:35.211485+00', NULL, '2026-01-13 18:17:35.211484+00', '2026-01-13 18:17:35.211485+00', false, NULL, NULL, NULL),
('981ab899-3df6-49b6-9d3c-3d12560b19c8', 'Hoang Ngoc Anh', '+84052018092', 'hoang.ngoc.anh6154@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '17450513', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/779.jpg', '2026-01-13 18:17:35.211823+00', NULL, '2026-01-13 18:17:35.211822+00', '2026-01-13 18:17:35.211823+00', false, NULL, NULL, NULL),
('99c4c633-0301-44bf-b550-e311f9dc7d0d', 'Phan Duc Son', '+84085066399', 'phan.duc.son5071@outlook.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '34624312', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/292.jpg', '2026-01-13 18:17:35.211843+00', NULL, '2026-01-13 18:17:35.211842+00', '2026-01-13 18:17:35.211843+00', false, NULL, NULL, NULL),
('9effd470-f238-4fef-890d-5a8e53a74d2a', 'Ngo Duc Hai', '+84034750931', 'ngo.duc.hai1876@yahoo.com', 'Offline', 'Rejected', NULL, NULL, NULL, NULL, NULL, NULL, '61502191', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/800.jpg', NULL, 'Invalid vehicle registration', '2026-01-13 18:17:35.211756+00', '2026-01-13 18:17:35.211758+00', false, NULL, NULL, NULL),
('a4ac76e2-7ed4-4d2f-baf8-3b04beae54f1', 'Ho Ngoc Nam', '+84036053049', 'ho.ngoc.nam4797@yahoo.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '70080780', NULL, '2026-01-13 18:17:35.211489+00', NULL, '2026-01-13 18:17:35.211487+00', '2026-01-13 18:17:35.211489+00', false, NULL, NULL, NULL),
('a5622cdb-1243-440c-a184-1f9e11ccfebd', 'Truong Thi Tien', '+84083467871', 'truong.thi.tien8607@outlook.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '72061360', NULL, NULL, NULL, '2026-01-13 18:17:35.211855+00', '2026-01-13 18:17:35.211855+00', false, NULL, NULL, NULL),
('a9e6d43c-ac5b-43f5-a2c3-2d9297f53279', 'Ngo Thi Nam', '+84096409608', 'ngo.thi.nam5412@email.vn', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '09962044', NULL, NULL, NULL, '2026-01-13 18:17:35.211749+00', '2026-01-13 18:17:35.211749+00', false, NULL, NULL, NULL),
('abe59204-6e2e-4316-92dd-d001aa0e52b9', 'Nguyen Hoang Huong', '+84051580512', 'nguyen.hoang.huong127@outlook.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '43859078', NULL, NULL, NULL, '2026-01-13 18:17:35.211736+00', '2026-01-13 18:17:35.211736+00', false, NULL, NULL, NULL),
('ac254e16-98ce-43b1-8ddb-a002407311b3', 'Nguyen Minh Hung', '+84090344970', 'nguyen.minh.hung872@gmail.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '96887282', NULL, '2026-01-13 18:17:35.211798+00', NULL, '2026-01-13 18:17:35.211796+00', '2026-01-13 18:17:35.211798+00', false, NULL, NULL, NULL),
('af79e7bf-6ada-451f-9152-36962f330f41', 'Luong Duc Son', '+84057153412', 'luong.duc.son9310@gmail.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '35408840', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/1214.jpg', NULL, NULL, '2026-01-13 18:17:35.211851+00', '2026-01-13 18:17:35.211853+00', false, NULL, NULL, NULL),
('b6b0ae8b-62c0-4414-b23c-84366a75d6fc', 'Ho Hong Khoa', '+84078400051', 'ho.hong.khoa3982@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/419.jpg', '2026-01-13 18:17:35.211806+00', NULL, '2026-01-13 18:17:35.211805+00', '2026-01-13 18:17:35.211806+00', false, NULL, NULL, NULL),
('beb8cf3b-ab95-4c62-a1d4-abffea99130c', 'Le Hong Tuan', '+84050762808', 'le.hong.tuan1824@outlook.com', 'Offline', 'Rejected', NULL, NULL, NULL, NULL, NULL, NULL, '55000922', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/38.jpg', NULL, 'Invalid vehicle registration', '2026-01-13 18:17:35.211714+00', '2026-01-13 18:17:35.211716+00', false, NULL, NULL, NULL),
('c7e027ca-c6ec-40d4-b5d3-a23832eeb3fc', 'Vu Thi Phuc', '+84030874371', 'vu.thi.phuc6430@gmail.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '17435024', NULL, '2026-01-13 18:17:35.211696+00', NULL, '2026-01-13 18:17:35.211694+00', '2026-01-13 18:17:35.211696+00', false, NULL, NULL, NULL),
('cb691df6-1288-4fd9-be2b-c36467f9059a', 'Dang Duc Diem', '+84095270683', 'dang.duc.diem2581@gmail.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-01-13 18:17:35.211846+00', NULL, '2026-01-13 18:17:35.211845+00', '2026-01-13 18:17:35.211846+00', false, NULL, NULL, NULL),
('d7c07e22-5952-4559-82dc-77a9bbb650c0', 'Ho Ngoc Phuong', '+84057318940', 'ho.ngoc.phuong3170@yahoo.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/995.jpg', '2026-01-13 18:17:35.211766+00', NULL, '2026-01-13 18:17:35.211765+00', '2026-01-13 18:17:35.211767+00', false, NULL, NULL, NULL),
('deea9dce-8b3a-46c5-82e2-7cac7f40ba0b', 'Vu Hong Nga', '+84075665528', 'vu.hong.nga3481@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-01-13 18:17:35.211744+00', NULL, '2026-01-13 18:17:35.211742+00', '2026-01-13 18:17:35.211744+00', false, NULL, NULL, NULL),
('df2ea5e6-d073-4599-800b-e7f9699f29cd', 'Bui Minh Phuong', '+84083774307', 'bui.minh.phuong7134@gmail.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-01-13 18:17:35.211477+00', NULL, '2026-01-13 18:17:35.211476+00', '2026-01-13 18:17:35.211477+00', false, NULL, NULL, NULL),
('e2f65f46-e42b-47a2-b14c-be33fc790873', 'Ngo Hong Tuan', '+84080384383', 'ngo.hong.tuan410@outlook.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '81556814', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/422.jpg', '2026-01-13 18:17:35.211817+00', NULL, '2026-01-13 18:17:35.211815+00', '2026-01-13 18:17:35.211817+00', false, NULL, NULL, NULL),
('e8bdbae9-501a-43bd-82df-652341e08adc', 'Bui Hoang Nga', '+84080683494', 'bui.hoang.nga1360@outlook.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '48108994', NULL, NULL, NULL, '2026-01-13 18:17:35.211704+00', '2026-01-13 18:17:35.211704+00', false, NULL, NULL, NULL),
('ea9f7833-a520-4f05-908e-747a4a433c81', 'Ngo Van Son', '+84056123589', 'ngo.van.son8149@outlook.com', 'Offline', 'Rejected', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'Invalid vehicle registration', '2026-01-13 18:17:35.21172+00', '2026-01-13 18:17:35.211722+00', false, NULL, NULL, NULL),
('f153eb9c-f915-4f81-8ddf-0427496a34bc', 'Vu Hong Huy', '+84076196982', 'vu.hong.huy3213@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-01-13 18:17:35.21184+00', NULL, '2026-01-13 18:17:35.211839+00', '2026-01-13 18:17:35.21184+00', false, NULL, NULL, NULL),
('f6130067-2a9b-4758-9de6-25438bf39f56', 'Luong Thi Ha', '+84085998368', 'luong.thi.ha8246@email.vn', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, '93873765', 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/658.jpg', '2026-01-13 18:17:35.21177+00', NULL, '2026-01-13 18:17:35.211768+00', '2026-01-13 18:17:35.21177+00', false, NULL, NULL, NULL),
('f85bbd3b-0d2b-480e-848c-5fcaacfe5455', 'Ho Hoang Thuy', '+84071840599', 'ho.hoang.thuy5544@yahoo.com', 'Offline', 'Pending', NULL, NULL, NULL, NULL, NULL, NULL, '56710997', NULL, NULL, NULL, '2026-01-13 18:17:35.211785+00', '2026-01-13 18:17:35.211785+00', false, NULL, NULL, NULL),
('fb105183-9e4d-4fee-9fe1-1bb1277e66f0', 'Pham Hoang Lan', '+84056888062', 'pham.hoang.lan8728@outlook.com', 'Offline', 'Verified', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/777.jpg', '2026-01-13 18:17:35.211699+00', NULL, '2026-01-13 18:17:35.211698+00', '2026-01-13 18:17:35.2117+00', false, NULL, NULL, NULL)
ON CONFLICT ("Id") DO NOTHING;
