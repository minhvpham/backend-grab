#!/bin/bash

export PGPASSWORD="1"

pg_dump -U postgres -h localhost -d postgres >dump/db/db_dump.sql
