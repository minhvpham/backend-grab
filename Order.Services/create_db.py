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
    
    # Connect to default 'postgres' db
    print(f"Connecting to postgres host: {host}")
    try:
        conn = psycopg2.connect(user=user, password=password, host=host, port=port, dbname='postgres')
        conn.set_isolation_level(ISOLATION_LEVEL_AUTOCOMMIT)
        cur = conn.cursor()
        
        # Check if db exists
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
        # Build phase might not have DB ready, or credentials wrong. 
        # We don't exit(1) here to allow retry or manual intervention, 
        # but Alembic will likely fail next if DB is missing.
        sys.exit(1)

if __name__ == "__main__":
    # Simple retry logic
    for i in range(5):
        try:
            create_db()
            break
        except Exception:
            print("Retrying DB creation in 2s...")
            time.sleep(2)
