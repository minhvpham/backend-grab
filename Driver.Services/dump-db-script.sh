#!/bin/bash

export POSTGRES_PASSWORD="postgres"
export POSTGRES_DB="driver_services"
export POSTGRES_USER="postgres"
export POSTGRES_HOST="localhost"
export POSTGRES_PORT="5433"

pg_dump -U $POSTGRES_USER -h $POSTGRES_HOST -p $POSTGRES_PORT -d $POSTGRES_DB >dump/db/db_dump.sql
