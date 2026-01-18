#!/bin/sh
set -e

echo "Ensuring database exists..."
cat <<EOF > create_db.py
import psycopg2
from psycopg2.extensions import ISOLATION_LEVEL_AUTOCOMMIT
import os
import sys
import time

def create_db():
    user = os.getenv("POSTGRES_USER", "postgres")
    password = os.getenv("POSTGRES_PASSWORD", "united_password")
    host = os.getenv("POSTGRES_HOST", "postgres")
    port = os.getenv("POSTGRES_PORT", "5432")
    target_db = os.getenv("POSTGRES_DB", "order_service_db")
    
    print(f"Connecting to postgres host: {host}")
    try:
        conn = psycopg2.connect(user=user, password=password, host=host, port=port, dbname='postgres')
        conn.set_isolation_level(ISOLATION_LEVEL_AUTOCOMMIT)
        cur = conn.cursor()
        
        cur.execute(f"SELECT 1 FROM pg_catalog.pg_database WHERE datname = '{target_db}'")
        exists = cur.fetchone()
        
        if not exists:
            print(f"Database '{target_db}' does not exist. Creating...")
            cur.execute(f"CREATE DATABASE {target_db}")
            print(f"Database '{target_db}' created successfully.")
        else:
            print(f"Database '{target_db}' already exists.")
            
        cur.close()
        conn.close()
    except Exception as e:
        print(f"Error checking/creating database: {e}")
        # Allow proceed, maybe created by other means
        pass

if __name__ == "__main__":
    for i in range(5):
        try:
            create_db()
            break
        except Exception:
            print("Retrying DB creation in 2s...")
            time.sleep(2)
EOF

python create_db.py

echo "Running database migrations..."
alembic upgrade head

# Start application
echo "Starting application..."
exec uvicorn app.main:app --host 0.0.0.0 --port 8000
