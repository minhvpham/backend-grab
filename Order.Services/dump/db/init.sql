--
-- PostgreSQL database dump
--


-- Dumped from database version 16.11 (Debian 16.11-1.pgdg13+1)
-- Dumped by pg_dump version 18.1

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

ALTER TABLE IF EXISTS ONLY public.orders DROP CONSTRAINT IF EXISTS orders_user_id_fkey;
ALTER TABLE IF EXISTS ONLY public.order_items DROP CONSTRAINT IF EXISTS order_items_order_id_fkey;
DROP INDEX IF EXISTS public.idx_profiles_phone;
DROP INDEX IF EXISTS public.idx_profiles_email;
DROP INDEX IF EXISTS public.idx_orders_user_id;
DROP INDEX IF EXISTS public.idx_orders_status;
DROP INDEX IF EXISTS public.idx_orders_restaurant_id;
DROP INDEX IF EXISTS public.idx_orders_driver_id;
DROP INDEX IF EXISTS public.idx_order_items_order_id;
ALTER TABLE IF EXISTS ONLY public.profiles DROP CONSTRAINT IF EXISTS profiles_pkey;
ALTER TABLE IF EXISTS ONLY public.profiles DROP CONSTRAINT IF EXISTS profiles_phone_key;
ALTER TABLE IF EXISTS ONLY public.profiles DROP CONSTRAINT IF EXISTS profiles_email_key;
ALTER TABLE IF EXISTS ONLY public.orders DROP CONSTRAINT IF EXISTS orders_pkey;
ALTER TABLE IF EXISTS ONLY public.order_items DROP CONSTRAINT IF EXISTS order_items_pkey;
DROP TABLE IF EXISTS public.profiles;
DROP TABLE IF EXISTS public.orders;
DROP TABLE IF EXISTS public.order_items;
SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: order_items; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.order_items (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    order_id uuid NOT NULL,
    product_id uuid NOT NULL,
    product_name character varying(255) NOT NULL,
    quantity integer DEFAULT 1 NOT NULL,
    unit_price numeric(12,2) NOT NULL,
    note text,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- Name: orders; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.orders (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id character varying(255) NOT NULL,
    restaurant_id uuid NOT NULL,
    driver_id uuid,
    status character varying(30) DEFAULT 'pending'::character varying NOT NULL,
    payment_status character varying(20) DEFAULT 'unpaid'::character varying NOT NULL,
    payment_method character varying(50),
    delivery_address text NOT NULL,
    delivery_note text,
    subtotal numeric(12,2) DEFAULT 0,
    delivery_fee numeric(12,2) DEFAULT 0,
    discount numeric(12,2) DEFAULT 0,
    total_amount numeric(12,2) DEFAULT 0,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- Name: profiles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.profiles (
    user_id character varying(255) NOT NULL,
    name character varying(255) NOT NULL,
    email character varying(255) NOT NULL,
    phone character varying(20) NOT NULL,
    role character varying(20) DEFAULT 'user'::character varying NOT NULL,
    avatar character varying(500),
    address text,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


--
-- Data for Name: order_items; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.order_items (id, order_id, product_id, product_name, quantity, unit_price, note, created_at) FROM stdin;
880e8400-e29b-41d4-a716-446655440001	660e8400-e29b-41d4-a716-446655440001	990e8400-e29b-41d4-a716-446655440001	Phở Bò Đặc Biệt	2	50000.00	\N	2026-01-15 13:10:00.784051
880e8400-e29b-41d4-a716-446655440002	660e8400-e29b-41d4-a716-446655440001	990e8400-e29b-41d4-a716-446655440002	Nước Chanh	2	25000.00	\N	2026-01-15 13:10:00.784051
880e8400-e29b-41d4-a716-446655440003	660e8400-e29b-41d4-a716-446655440002	990e8400-e29b-41d4-a716-446655440003	Bún Chả Hà Nội	2	60000.00	\N	2026-01-15 13:10:00.784051
880e8400-e29b-41d4-a716-446655440004	660e8400-e29b-41d4-a716-446655440002	990e8400-e29b-41d4-a716-446655440004	Trà Đá	4	5000.00	\N	2026-01-15 13:10:00.784051
880e8400-e29b-41d4-a716-446655440005	660e8400-e29b-41d4-a716-446655440002	990e8400-e29b-41d4-a716-446655440005	Nem Rán	2	30000.00	\N	2026-01-15 13:10:00.784051
880e8400-e29b-41d4-a716-446655440006	660e8400-e29b-41d4-a716-446655440003	990e8400-e29b-41d4-a716-446655440006	Cơm Sườn Bì Chả	1	55000.00	\N	2026-01-15 13:10:00.784051
880e8400-e29b-41d4-a716-446655440007	660e8400-e29b-41d4-a716-446655440003	990e8400-e29b-41d4-a716-446655440007	Nước Mía	1	30000.00	\N	2026-01-15 13:10:00.784051
880e8400-e29b-41d4-a716-446655440008	660e8400-e29b-41d4-a716-446655440004	990e8400-e29b-41d4-a716-446655440008	Bánh Mì Thịt Nướng	3	30000.00	\N	2026-01-15 13:10:00.784051
880e8400-e29b-41d4-a716-446655440009	660e8400-e29b-41d4-a716-446655440004	990e8400-e29b-41d4-a716-446655440009	Trà Sữa Trân Châu	2	35000.00	\N	2026-01-15 13:10:00.784051
880e8400-e29b-41d4-a716-446655440010	660e8400-e29b-41d4-a716-446655440005	990e8400-e29b-41d4-a716-446655440010	Gỏi Cuốn	4	15000.00	\N	2026-01-15 13:10:00.784051
880e8400-e29b-41d4-a716-446655440011	660e8400-e29b-41d4-a716-446655440005	990e8400-e29b-41d4-a716-446655440011	Nước Ép Cam	2	25000.00	\N	2026-01-15 13:10:00.784051
\.


--
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.orders (id, user_id, restaurant_id, driver_id, status, payment_status, payment_method, delivery_address, delivery_note, subtotal, delivery_fee, discount, total_amount, created_at, updated_at) FROM stdin;
660e8400-e29b-41d4-a716-446655440001	auth_user_001	770e8400-e29b-41d4-a716-446655440001	\N	delivered	paid	cash	123 Nguyễn Huệ, Quận 1, TP.HCM	\N	150000.00	15000.00	0.00	165000.00	2026-01-15 13:10:00.782125	2026-01-15 13:10:00.782125
660e8400-e29b-41d4-a716-446655440002	auth_user_001	770e8400-e29b-41d4-a716-446655440001	\N	pending	unpaid	momo	123 Nguyễn Huệ, Quận 1, TP.HCM	\N	200000.00	15000.00	0.00	215000.00	2026-01-15 13:10:00.782125	2026-01-15 13:10:00.782125
660e8400-e29b-41d4-a716-446655440003	auth_user_002	770e8400-e29b-41d4-a716-446655440002	\N	preparing	paid	card	456 Lê Lợi, Quận 3, TP.HCM	\N	85000.00	15000.00	0.00	100000.00	2026-01-15 13:10:00.782125	2026-01-15 13:10:00.782125
660e8400-e29b-41d4-a716-446655440004	auth_user_001	770e8400-e29b-41d4-a716-446655440003	\N	delivering	paid	zalopay	789 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM	\N	120000.00	20000.00	0.00	140000.00	2026-01-15 13:10:00.782125	2026-01-15 13:10:00.782125
660e8400-e29b-41d4-a716-446655440005	auth_user_003	770e8400-e29b-41d4-a716-446655440001	\N	confirmed	paid	momo	321 Võ Văn Tần, Quận 3, TP.HCM	\N	95000.00	15000.00	0.00	110000.00	2026-01-15 13:10:00.782125	2026-01-15 13:10:00.782125
\.


--
-- Data for Name: profiles; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.profiles (user_id, name, email, phone, role, avatar, address, created_at, updated_at) FROM stdin;
auth_user_001	Nguyễn Văn A	nguyenvana@gmail.com	0901234567	user	\N	123 Nguyễn Huệ, Quận 1, TP.HCM	2026-01-15 13:10:00.780628	2026-01-15 13:10:00.780628
auth_user_002	Trần Thị B	tranthib@gmail.com	0912345678	user	\N	456 Lê Lợi, Quận 3, TP.HCM	2026-01-15 13:10:00.780628	2026-01-15 13:10:00.780628
auth_user_003	Lê Văn C	levanc@gmail.com	0923456789	user	\N	789 Trần Hưng Đạo, Quận 5, TP.HCM	2026-01-15 13:10:00.780628	2026-01-15 13:10:00.780628
auth_driver_001	Phạm Văn D	driver1@gmail.com	0934567890	shipper	\N	321 Võ Văn Tần, Quận 3, TP.HCM	2026-01-15 13:10:00.780628	2026-01-15 13:10:00.780628
auth_driver_002	Hoàng Thị E	driver2@gmail.com	0945678901	shipper	\N	654 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM	2026-01-15 13:10:00.780628	2026-01-15 13:10:00.780628
auth_admin_001	Admin System	admin@grabfood.vn	0900000000	admin	\N	HCM	2026-01-15 13:10:00.780628	2026-01-15 13:10:00.780628
\.


--
-- Name: order_items order_items_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT order_items_pkey PRIMARY KEY (id);


--
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (id);


--
-- Name: profiles profiles_email_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profiles
    ADD CONSTRAINT profiles_email_key UNIQUE (email);


--
-- Name: profiles profiles_phone_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profiles
    ADD CONSTRAINT profiles_phone_key UNIQUE (phone);


--
-- Name: profiles profiles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.profiles
    ADD CONSTRAINT profiles_pkey PRIMARY KEY (user_id);


--
-- Name: idx_order_items_order_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_order_items_order_id ON public.order_items USING btree (order_id);


--
-- Name: idx_orders_driver_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_orders_driver_id ON public.orders USING btree (driver_id);


--
-- Name: idx_orders_restaurant_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_orders_restaurant_id ON public.orders USING btree (restaurant_id);


--
-- Name: idx_orders_status; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_orders_status ON public.orders USING btree (status);


--
-- Name: idx_orders_user_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_orders_user_id ON public.orders USING btree (user_id);


--
-- Name: idx_profiles_email; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_profiles_email ON public.profiles USING btree (email);


--
-- Name: idx_profiles_phone; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_profiles_phone ON public.profiles USING btree (phone);


--
-- Name: order_items order_items_order_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT order_items_order_id_fkey FOREIGN KEY (order_id) REFERENCES public.orders(id) ON DELETE CASCADE;


--
-- Name: orders orders_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.profiles(user_id);


--
-- PostgreSQL database dump complete
--


