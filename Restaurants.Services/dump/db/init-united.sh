#!/bin/bash
set -e

# Run the restaurants service init script against the restaurants_services database
psql -U postgres -d restaurants_services -f /tmp/restaurants-init.sql
