#!/bin/bash

# ================= CONFIG =================
DB_USER="postgres"
DB_PASSWORD="password"
DB_NAME="mydatabase"
DB_PORT="5432"
CONTAINER_NAME="fastapi_postgres"

# Cấu hình web client (Adminer hoặc pgAdmin)
WEB_CLIENT="adminer"  # chọn "pgadmin" hoặc "adminer"
WEB_PORT="8080"

# ================= POSTGRES =================
# Kiểm tra nếu container PostgreSQL đã chạy
if [ $(docker ps -a -q -f name=$CONTAINER_NAME) ]; then
    echo "Container $CONTAINER_NAME đã tồn tại, dừng và xóa..."
    docker rm -f $CONTAINER_NAME
fi

# Chạy container PostgreSQL
docker run -d \
    --name $CONTAINER_NAME \
    -e POSTGRES_USER=$DB_USER \
    -e POSTGRES_PASSWORD=$DB_PASSWORD \
    -e POSTGRES_DB=$DB_NAME \
    -p $DB_PORT:5432 \
    postgres:15

echo "PostgreSQL đang chạy tại localhost:$DB_PORT với user=$DB_USER và database=$DB_NAME"

# ================= WEB CLIENT =================
if [ "$WEB_CLIENT" == "adminer" ]; then
    echo "Chạy Adminer (web client) tại http://localhost:$WEB_PORT"
    docker run -d --name adminer -p $WEB_PORT:8080 adminer
elif [ "$WEB_CLIENT" == "pgadmin" ]; then
    echo "Chạy pgAdmin (web client) tại http://localhost:$WEB_PORT"
    docker run -d \
        --name pgadmin \
        -p $WEB_PORT:80 \
        -e PGADMIN_DEFAULT_EMAIL=admin@admin.com \
        -e PGADMIN_DEFAULT_PASSWORD=admin \
        dpage/pgadmin4
else
    echo "Không hỗ trợ client web: $WEB_CLIENT"
fi
