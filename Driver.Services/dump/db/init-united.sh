#!/bin/bash
set -e

# Run the driver service init script against the driver_services database
psql -U postgres -d driver_services -f /tmp/driver-init.sql
