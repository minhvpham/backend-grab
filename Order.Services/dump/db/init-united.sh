#!/bin/bash
set -e

# Run the order service init script against the order_service_db
psql -U postgres -d order_service_db -f /tmp/order-init.sql
