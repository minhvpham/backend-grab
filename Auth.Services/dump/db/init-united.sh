#!/bin/bash
set -e

# Run the auth service init script against the auth_services database
psql -U postgres -d auth_services -f /tmp/auth-init.sql
