#!/bin/bash
# Script to dump database for sharing with team
# Similar to Driver service's dump script

CONTAINER_NAME="order-service-postgres"
DB_NAME="order_service_db"
DB_USER="postgres"
DUMP_PATH="./dump/db/db_dump.sql"

echo "Dumping database from $CONTAINER_NAME..."
docker exec -t $CONTAINER_NAME pg_dump -U $DB_USER $DB_NAME > $DUMP_PATH

echo "Database dumped to $DUMP_PATH"
echo "Done!"
