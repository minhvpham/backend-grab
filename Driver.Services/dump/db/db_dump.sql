--
-- PostgreSQL database dump
--

-- \restrict PBOdLp6plm8DydUnWPfdmxtWKxf4Ghh1C1divuNgHpflf6aFxkd3aS6Hu09SFdO

-- Dumped from database version 16.9 (Debian 16.9-1.pgdg130+1)
-- Dumped by pg_dump version 18.1

-- SET statement_timeout = 0;
-- SET lock_timeout = 0;
-- SET idle_in_transaction_session_timeout = 0;
-- SET transaction_timeout = 0;
-- SET client_encoding = 'UTF8';
-- SET standard_conforming_strings = on;
-- SELECT pg_catalog.set_config('search_path', '', false);
-- SET check_function_bodies = false;
-- SET xmloption = content;
-- SET client_min_messages = warning;
-- SET row_security = off;
--
-- SET default_tablespace = '';
--
-- SET default_table_access_method = heap;

--
-- Name: DriverLocations; Type: TABLE; Schema: public; Owner: postgres
--

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

--
-- Name: DriverWallets; Type: TABLE; Schema: public; Owner: postgres
--

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

--
-- Name: Drivers; Type: TABLE; Schema: public; Owner: postgres
--

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

--
-- Name: Transactions; Type: TABLE; Schema: public; Owner: postgres
--

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

--
-- Name: TripHistories; Type: TABLE; Schema: public; Owner: postgres
--

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

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Data for Name: DriverLocations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."DriverLocations" ("Id", "DriverId", "Latitude", "Longitude", "Accuracy", "Heading", "Speed", "Timestamp", "CreatedAt", "UpdatedAt", "Deleted", "DeletedAt", "UpdateByUserId", "UpdateByIdentityName") FROM stdin;
\.


--
-- Data for Name: DriverWallets; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."DriverWallets" ("Id", "DriverId", "Balance", "CashOnHand", "TotalEarnings", "TotalWithdrawn", "LastWithdrawalAt", "IsActive", "CreatedAt", "UpdatedAt", "Deleted", "DeletedAt", "UpdateByUserId", "UpdateByIdentityName") FROM stdin;
\.


--
-- Data for Name: Drivers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Drivers" ("Id", "FullName", "PhoneNumber", "Email", "Status", "VerificationStatus", "VehicleType", "LicensePlate", "VehicleBrand", "VehicleModel", "VehicleYear", "VehicleColor", "LicenseNumber", "ProfileImageUrl", "VerifiedAt", "RejectionReason", "CreatedAt", "UpdatedAt", "Deleted", "DeletedAt", "UpdateByUserId", "UpdateByIdentityName") FROM stdin;
0853d007-3ca5-4b21-8a05-371638cd57d3	Ngo Hoang Binh	+84073639689	ngo.hoang.binh5703@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	59118594	\N	2026-01-13 18:17:35.211837+00	\N	2026-01-13 18:17:35.211836+00	2026-01-13 18:17:35.211837+00	f	\N	\N	\N
125c5232-99f0-43e7-bc0b-6efb83b9277a	Ngo Thanh Yen	+84086840602	ngo.thanh.yen9817@outlook.com	Offline	Pending	\N	\N	\N	\N	\N	\N	65353007	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/800.jpg	\N	\N	2026-01-13 18:17:35.211861+00	2026-01-13 18:17:35.211862+00	f	\N	\N	\N
28546a19-406d-4f93-b35d-de57acf1b8b0	Tran Hong My	+84057267664	tran.hong.my1274@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	56175973	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/156.jpg	2026-01-13 18:17:35.211175+00	\N	2026-01-13 18:17:35.21044+00	2026-01-13 18:17:35.211453+00	f	\N	\N	\N
2cef4120-ed39-4f25-bf91-888ec7b4b54b	Tran Ngoc Thanh	+84089001824	tran.ngoc.thanh7013@gmail.com	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	\N	2026-01-13 18:17:35.211801+00	\N	2026-01-13 18:17:35.211799+00	2026-01-13 18:17:35.211801+00	f	\N	\N	\N
3472a608-0870-462f-a1ba-5efb21cbcace	Truong Quoc Minh	+84086670396	truong.quoc.minh3468@gmail.com	Offline	Verified	\N	\N	\N	\N	\N	\N	44048490	\N	2026-01-13 18:17:35.211834+00	\N	2026-01-13 18:17:35.211832+00	2026-01-13 18:17:35.211834+00	f	\N	\N	\N
36054f77-0c8b-4bea-b2c2-b4b659a5c94f	Vo Ngoc Yen	+84071331296	vo.ngoc.yen3308@outlook.com	Offline	Rejected	\N	\N	\N	\N	\N	\N	46361030	\N	\N	Vehicle does not meet requirements	2026-01-13 18:17:35.211491+00	2026-01-13 18:17:35.211641+00	f	\N	\N	\N
3f666688-cbaf-4b04-9382-4589e677a1ca	Dang Van Phuc	+84050199797	dang.van.phuc8920@email.vn	Offline	Pending	\N	\N	\N	\N	\N	\N	98408824	\N	\N	\N	2026-01-13 18:17:35.211775+00	2026-01-13 18:17:35.211775+00	f	\N	\N	\N
461f2337-50a5-4e2b-8c9e-69a4b01f48d3	Vo Thanh Cuong	+84079943107	vo.thanh.cuong3357@outlook.com	Offline	Pending	\N	\N	\N	\N	\N	\N	91633468	\N	\N	\N	2026-01-13 18:17:35.211711+00	2026-01-13 18:17:35.211711+00	f	\N	\N	\N
50fd1821-ad6d-4210-acdf-a14d34744956	Vo Minh Anh	+84051605216	vo.minh.anh903@gmail.com	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	\N	2026-01-13 18:17:35.21176+00	\N	2026-01-13 18:17:35.211759+00	2026-01-13 18:17:35.21176+00	f	\N	\N	\N
5540e085-5c96-4a77-ba0d-733ebc7fb7be	Hoang Minh My	+84030889246	hoang.minh.my867@outlook.com	Offline	Pending	\N	\N	\N	\N	\N	\N	29746827	\N	\N	\N	2026-01-13 18:17:35.211788+00	2026-01-13 18:17:35.211788+00	f	\N	\N	\N
55974ac1-a797-4c14-960d-3dedc8f28324	Tran Ngoc Dung	+84093386784	tran.ngoc.dung4363@yahoo.com	Offline	Pending	\N	\N	\N	\N	\N	\N	45586050	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/242.jpg	\N	\N	2026-01-13 18:17:35.211463+00	2026-01-13 18:17:35.211474+00	f	\N	\N	\N
560882ac-457f-4764-ae7d-4dd2e9cdbae5	Trinh Thanh Mai	+84033534543	trinh.thanh.mai7650@gmail.com	Offline	Verified	\N	\N	\N	\N	\N	\N	22204082	\N	2026-01-13 18:17:35.211763+00	\N	2026-01-13 18:17:35.211762+00	2026-01-13 18:17:35.211763+00	f	\N	\N	\N
57327cb8-0563-4f6b-a679-7dae9a4ec3d6	Ly Duc Anh	+84083525606	ly.duc.anh3224@outlook.com	Offline	Verified	\N	\N	\N	\N	\N	\N	61814728	\N	2026-01-13 18:17:35.211826+00	\N	2026-01-13 18:17:35.211825+00	2026-01-13 18:17:35.211826+00	f	\N	\N	\N
64c27678-d3be-4b43-a123-f9aa7415c935	Do Xuan Son	+84094199900	do.xuan.son8845@yahoo.com	Offline	Verified	\N	\N	\N	\N	\N	\N	23820657	\N	2026-01-13 18:17:35.211783+00	\N	2026-01-13 18:17:35.211781+00	2026-01-13 18:17:35.211783+00	f	\N	\N	\N
76805067-db43-4bc9-9cd6-8e9c06078ab3	Ly Quoc Duc	+84090971585	ly.quoc.duc3908@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	42687567	\N	2026-01-13 18:17:35.21178+00	\N	2026-01-13 18:17:35.211778+00	2026-01-13 18:17:35.21178+00	f	\N	\N	\N
7889b235-f5c7-450f-a009-9692c1e79d4f	Luong Minh Tuan	+84097710685	luong.minh.tuan5976@gmail.com	Offline	Verified	\N	\N	\N	\N	\N	\N	77333806	\N	2026-01-13 18:17:35.211692+00	\N	2026-01-13 18:17:35.211688+00	2026-01-13 18:17:35.211692+00	f	\N	\N	\N
78b636ed-1bb1-46ee-871c-8243a30adb3f	Pham Van Quynh	+84036980269	pham.van.quynh381@outlook.com	Offline	Verified	\N	\N	\N	\N	\N	\N	70541233	\N	2026-01-13 18:17:35.211725+00	\N	2026-01-13 18:17:35.211724+00	2026-01-13 18:17:35.211725+00	f	\N	\N	\N
78fbfeb2-3621-48a7-a459-fddc4c85706c	Do Quoc Long	+84092078501	do.quoc.long887@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	\N	2026-01-13 18:17:35.211702+00	\N	2026-01-13 18:17:35.211701+00	2026-01-13 18:17:35.211702+00	f	\N	\N	\N
7b0416f5-1b2f-4515-a86b-b10961dd5912	Ho Quoc Linh	+84070243400	ho.quoc.linh8520@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/155.jpg	2026-01-13 18:17:35.211803+00	\N	2026-01-13 18:17:35.211802+00	2026-01-13 18:17:35.211803+00	f	\N	\N	\N
7ed92b85-bb77-4905-9985-1072e68fa403	Vo Quoc Ngoc	+84056054400	vo.quoc.ngoc8956@gmail.com	Offline	Pending	\N	\N	\N	\N	\N	\N	\N	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/763.jpg	\N	\N	2026-01-13 18:17:35.211717+00	2026-01-13 18:17:35.211719+00	f	\N	\N	\N
85ce8be8-61bc-469a-923d-2edb790ce2b7	Hoang Thanh Dung	+84087647431	hoang.thanh.dung7203@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	51288957	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/4.jpg	2026-01-13 18:17:35.211794+00	\N	2026-01-13 18:17:35.211793+00	2026-01-13 18:17:35.211794+00	f	\N	\N	\N
88602bf8-4770-4da3-aacb-23a6ef587d53	Ngo Hong Phong	+84088582472	ngo.hong.phong9928@outlook.com	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	\N	2026-01-13 18:17:35.21182+00	\N	2026-01-13 18:17:35.211818+00	2026-01-13 18:17:35.21182+00	f	\N	\N	\N
8cb0fb98-a8c2-4df2-b4a9-119091d2abd8	Truong Thanh Nga	+84080747712	truong.thanh.nga9079@outlook.com	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	\N	2026-01-13 18:17:35.211747+00	\N	2026-01-13 18:17:35.211745+00	2026-01-13 18:17:35.211747+00	f	\N	\N	\N
9359a861-5375-415d-92d9-bcbc550ed67a	Duong Xuan Hai	+84058321236	duong.xuan.hai5464@yahoo.com	Offline	Pending	\N	\N	\N	\N	\N	\N	73852069	\N	\N	\N	2026-01-13 18:17:35.21174+00	2026-01-13 18:17:35.21174+00	f	\N	\N	\N
93d38328-2151-4d96-ba43-adb846797330	Ly Minh Linh	+84090541700	ly.minh.linh5368@yahoo.com	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/517.jpg	2026-01-13 18:17:35.211859+00	\N	2026-01-13 18:17:35.211857+00	2026-01-13 18:17:35.211859+00	f	\N	\N	\N
93e6dc78-f152-489d-b558-a08c99f37401	Trinh Duc Lan	+84056688549	trinh.duc.lan4703@outlook.com	Offline	Verified	\N	\N	\N	\N	\N	\N	17128509	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/1168.jpg	2026-01-13 18:17:35.211482+00	\N	2026-01-13 18:17:35.21148+00	2026-01-13 18:17:35.211482+00	f	\N	\N	\N
97e51a86-02ec-4198-8baf-d6b7c12d435a	Hoang Thanh Huy	+84052906425	hoang.thanh.huy2759@yahoo.com	Offline	Verified	\N	\N	\N	\N	\N	\N	21527136	\N	2026-01-13 18:17:35.211485+00	\N	2026-01-13 18:17:35.211484+00	2026-01-13 18:17:35.211485+00	f	\N	\N	\N
981ab899-3df6-49b6-9d3c-3d12560b19c8	Hoang Ngoc Anh	+84052018092	hoang.ngoc.anh6154@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	17450513	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/779.jpg	2026-01-13 18:17:35.211823+00	\N	2026-01-13 18:17:35.211822+00	2026-01-13 18:17:35.211823+00	f	\N	\N	\N
99c4c633-0301-44bf-b550-e311f9dc7d0d	Phan Duc Son	+84085066399	phan.duc.son5071@outlook.com	Offline	Verified	\N	\N	\N	\N	\N	\N	34624312	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/292.jpg	2026-01-13 18:17:35.211843+00	\N	2026-01-13 18:17:35.211842+00	2026-01-13 18:17:35.211843+00	f	\N	\N	\N
9effd470-f238-4fef-890d-5a8e53a74d2a	Ngo Duc Hai	+84034750931	ngo.duc.hai1876@yahoo.com	Offline	Rejected	\N	\N	\N	\N	\N	\N	61502191	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/800.jpg	\N	Invalid vehicle registration	2026-01-13 18:17:35.211756+00	2026-01-13 18:17:35.211758+00	f	\N	\N	\N
a4ac76e2-7ed4-4d2f-baf8-3b04beae54f1	Ho Ngoc Nam	+84036053049	ho.ngoc.nam4797@yahoo.com	Offline	Verified	\N	\N	\N	\N	\N	\N	70080780	\N	2026-01-13 18:17:35.211489+00	\N	2026-01-13 18:17:35.211487+00	2026-01-13 18:17:35.211489+00	f	\N	\N	\N
a5622cdb-1243-440c-a184-1f9e11ccfebd	Truong Thi Tien	+84083467871	truong.thi.tien8607@outlook.com	Offline	Pending	\N	\N	\N	\N	\N	\N	72061360	\N	\N	\N	2026-01-13 18:17:35.211855+00	2026-01-13 18:17:35.211855+00	f	\N	\N	\N
a9e6d43c-ac5b-43f5-a2c3-2d9297f53279	Ngo Thi Nam	+84096409608	ngo.thi.nam5412@email.vn	Offline	Pending	\N	\N	\N	\N	\N	\N	09962044	\N	\N	\N	2026-01-13 18:17:35.211749+00	2026-01-13 18:17:35.211749+00	f	\N	\N	\N
abe59204-6e2e-4316-92dd-d001aa0e52b9	Nguyen Hoang Huong	+84051580512	nguyen.hoang.huong127@outlook.com	Offline	Pending	\N	\N	\N	\N	\N	\N	43859078	\N	\N	\N	2026-01-13 18:17:35.211736+00	2026-01-13 18:17:35.211736+00	f	\N	\N	\N
ac254e16-98ce-43b1-8ddb-a002407311b3	Nguyen Minh Hung	+84090344970	nguyen.minh.hung872@gmail.com	Offline	Verified	\N	\N	\N	\N	\N	\N	96887282	\N	2026-01-13 18:17:35.211798+00	\N	2026-01-13 18:17:35.211796+00	2026-01-13 18:17:35.211798+00	f	\N	\N	\N
af79e7bf-6ada-451f-9152-36962f330f41	Luong Duc Son	+84057153412	luong.duc.son9310@gmail.com	Offline	Pending	\N	\N	\N	\N	\N	\N	35408840	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/1214.jpg	\N	\N	2026-01-13 18:17:35.211851+00	2026-01-13 18:17:35.211853+00	f	\N	\N	\N
b6b0ae8b-62c0-4414-b23c-84366a75d6fc	Ho Hong Khoa	+84078400051	ho.hong.khoa3982@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/419.jpg	2026-01-13 18:17:35.211806+00	\N	2026-01-13 18:17:35.211805+00	2026-01-13 18:17:35.211806+00	f	\N	\N	\N
beb8cf3b-ab95-4c62-a1d4-abffea99130c	Le Hong Tuan	+84050762808	le.hong.tuan1824@outlook.com	Offline	Rejected	\N	\N	\N	\N	\N	\N	55000922	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/38.jpg	\N	Invalid vehicle registration	2026-01-13 18:17:35.211714+00	2026-01-13 18:17:35.211716+00	f	\N	\N	\N
c7e027ca-c6ec-40d4-b5d3-a23832eeb3fc	Vu Thi Phuc	+84030874371	vu.thi.phuc6430@gmail.com	Offline	Verified	\N	\N	\N	\N	\N	\N	17435024	\N	2026-01-13 18:17:35.211696+00	\N	2026-01-13 18:17:35.211694+00	2026-01-13 18:17:35.211696+00	f	\N	\N	\N
cb691df6-1288-4fd9-be2b-c36467f9059a	Dang Duc Diem	+84095270683	dang.duc.diem2581@gmail.com	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	\N	2026-01-13 18:17:35.211846+00	\N	2026-01-13 18:17:35.211845+00	2026-01-13 18:17:35.211846+00	f	\N	\N	\N
d7c07e22-5952-4559-82dc-77a9bbb650c0	Ho Ngoc Phuong	+84057318940	ho.ngoc.phuong3170@yahoo.com	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/995.jpg	2026-01-13 18:17:35.211766+00	\N	2026-01-13 18:17:35.211765+00	2026-01-13 18:17:35.211767+00	f	\N	\N	\N
deea9dce-8b3a-46c5-82e2-7cac7f40ba0b	Vu Hong Nga	+84075665528	vu.hong.nga3481@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	\N	2026-01-13 18:17:35.211744+00	\N	2026-01-13 18:17:35.211742+00	2026-01-13 18:17:35.211744+00	f	\N	\N	\N
df2ea5e6-d073-4599-800b-e7f9699f29cd	Bui Minh Phuong	+84083774307	bui.minh.phuong7134@gmail.com	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	\N	2026-01-13 18:17:35.211477+00	\N	2026-01-13 18:17:35.211476+00	2026-01-13 18:17:35.211477+00	f	\N	\N	\N
e2f65f46-e42b-47a2-b14c-be33fc790873	Ngo Hong Tuan	+84080384383	ngo.hong.tuan410@outlook.com	Offline	Verified	\N	\N	\N	\N	\N	\N	81556814	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/422.jpg	2026-01-13 18:17:35.211817+00	\N	2026-01-13 18:17:35.211815+00	2026-01-13 18:17:35.211817+00	f	\N	\N	\N
e8bdbae9-501a-43bd-82df-652341e08adc	Bui Hoang Nga	+84080683494	bui.hoang.nga1360@outlook.com	Offline	Pending	\N	\N	\N	\N	\N	\N	48108994	\N	\N	\N	2026-01-13 18:17:35.211704+00	2026-01-13 18:17:35.211704+00	f	\N	\N	\N
ea9f7833-a520-4f05-908e-747a4a433c81	Ngo Van Son	+84056123589	ngo.van.son8149@outlook.com	Offline	Rejected	\N	\N	\N	\N	\N	\N	\N	\N	\N	Invalid vehicle registration	2026-01-13 18:17:35.21172+00	2026-01-13 18:17:35.211722+00	f	\N	\N	\N
f153eb9c-f915-4f81-8ddf-0427496a34bc	Vu Hong Huy	+84076196982	vu.hong.huy3213@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	\N	2026-01-13 18:17:35.21184+00	\N	2026-01-13 18:17:35.211839+00	2026-01-13 18:17:35.21184+00	f	\N	\N	\N
f6130067-2a9b-4758-9de6-25438bf39f56	Luong Thi Ha	+84085998368	luong.thi.ha8246@email.vn	Offline	Verified	\N	\N	\N	\N	\N	\N	93873765	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/658.jpg	2026-01-13 18:17:35.21177+00	\N	2026-01-13 18:17:35.211768+00	2026-01-13 18:17:35.21177+00	f	\N	\N	\N
f85bbd3b-0d2b-480e-848c-5fcaacfe5455	Ho Hoang Thuy	+84071840599	ho.hoang.thuy5544@yahoo.com	Offline	Pending	\N	\N	\N	\N	\N	\N	56710997	\N	\N	\N	2026-01-13 18:17:35.211785+00	2026-01-13 18:17:35.211785+00	f	\N	\N	\N
fb105183-9e4d-4fee-9fe1-1bb1277e66f0	Pham Hoang Lan	+84056888062	pham.hoang.lan8728@outlook.com	Offline	Verified	\N	\N	\N	\N	\N	\N	\N	https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/777.jpg	2026-01-13 18:17:35.211699+00	\N	2026-01-13 18:17:35.211698+00	2026-01-13 18:17:35.2117+00	f	\N	\N	\N
\.


--
-- Data for Name: Transactions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Transactions" ("Id", "Type", "Amount", "BalanceBefore", "BalanceAfter", "OrderId", "Reference", "Description", "CreatedAt", "WalletId") FROM stdin;
\.


--
-- Data for Name: TripHistories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."TripHistories" ("Id", "DriverId", "OrderId", "Status", "PickupAddress", "PickupLatitude", "PickupLongitude", "DeliveryAddress", "DeliveryLatitude", "DeliveryLongitude", "DistanceKm", "DurationMinutes", "Fare", "CashCollected", "AssignedAt", "AcceptedAt", "PickedUpAt", "DeliveredAt", "CancelledAt", "CancellationReason", "FailureReason", "CustomerNotes", "DriverNotes", "CustomerRating", "CustomerFeedback", "CreatedAt", "UpdatedAt", "Deleted", "DeletedAt", "UpdateByUserId", "UpdateByIdentityName") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260112174418_InitialCreate	8.0.11
\.


--
-- Name: DriverLocations PK_DriverLocations; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DriverLocations"
    ADD CONSTRAINT "PK_DriverLocations" PRIMARY KEY ("Id");


--
-- Name: DriverWallets PK_DriverWallets; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DriverWallets"
    ADD CONSTRAINT "PK_DriverWallets" PRIMARY KEY ("Id");


--
-- Name: Drivers PK_Drivers; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Drivers"
    ADD CONSTRAINT "PK_Drivers" PRIMARY KEY ("Id");


--
-- Name: Transactions PK_Transactions; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Transactions"
    ADD CONSTRAINT "PK_Transactions" PRIMARY KEY ("Id");


--
-- Name: TripHistories PK_TripHistories; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TripHistories"
    ADD CONSTRAINT "PK_TripHistories" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_DriverLocations_DriverId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_DriverLocations_DriverId" ON public."DriverLocations" USING btree ("DriverId");


--
-- Name: IX_DriverLocations_Latitude_Longitude; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_DriverLocations_Latitude_Longitude" ON public."DriverLocations" USING btree ("Latitude", "Longitude");


--
-- Name: IX_DriverLocations_Timestamp; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_DriverLocations_Timestamp" ON public."DriverLocations" USING btree ("Timestamp");


--
-- Name: IX_DriverWallets_DriverId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_DriverWallets_DriverId" ON public."DriverWallets" USING btree ("DriverId");


--
-- Name: IX_DriverWallets_IsActive; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_DriverWallets_IsActive" ON public."DriverWallets" USING btree ("IsActive");


--
-- Name: IX_Drivers_Email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_Drivers_Email" ON public."Drivers" USING btree ("Email");


--
-- Name: IX_Drivers_Status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Drivers_Status" ON public."Drivers" USING btree ("Status");


--
-- Name: IX_Drivers_VerificationStatus; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Drivers_VerificationStatus" ON public."Drivers" USING btree ("VerificationStatus");


--
-- Name: IX_Transactions_CreatedAt; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Transactions_CreatedAt" ON public."Transactions" USING btree ("CreatedAt");


--
-- Name: IX_Transactions_OrderId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Transactions_OrderId" ON public."Transactions" USING btree ("OrderId");


--
-- Name: IX_Transactions_Type; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Transactions_Type" ON public."Transactions" USING btree ("Type");


--
-- Name: IX_Transactions_WalletId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Transactions_WalletId" ON public."Transactions" USING btree ("WalletId");


--
-- Name: IX_TripHistories_AssignedAt; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TripHistories_AssignedAt" ON public."TripHistories" USING btree ("AssignedAt");


--
-- Name: IX_TripHistories_DeliveredAt; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TripHistories_DeliveredAt" ON public."TripHistories" USING btree ("DeliveredAt");


--
-- Name: IX_TripHistories_DriverId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TripHistories_DriverId" ON public."TripHistories" USING btree ("DriverId");


--
-- Name: IX_TripHistories_OrderId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_TripHistories_OrderId" ON public."TripHistories" USING btree ("OrderId");


--
-- Name: IX_TripHistories_Status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TripHistories_Status" ON public."TripHistories" USING btree ("Status");


--
-- Name: Transactions FK_Transactions_DriverWallets_WalletId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Transactions"
    ADD CONSTRAINT "FK_Transactions_DriverWallets_WalletId" FOREIGN KEY ("WalletId") REFERENCES public."DriverWallets"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

-- \unrestrict PBOdLp4plm8DydUnWPfdmxtWKxf4Ghh1C1divuNgHpflf6aFxkd3aS6Hu09SFdO

