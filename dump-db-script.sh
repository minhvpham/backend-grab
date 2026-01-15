# Unified Database Dump Script

This script allows you to dump data from multiple services in the united backend setup.

## Usage

### Interactive Mode
```bash
./dump-db-script.sh
```
Shows a menu to select which services to dump.

### Command Line Mode
```bash
# Dump specific services
./dump-db-script.sh driver restaurants

# Dump all services
./dump-db-script.sh driver restaurants auth order
```

## Services Available

- **driver**: Driver.Services database → `Driver.Services/dump/db/db_dump_clean.sql`
- **restaurants**: Restaurants.Services database → `Restaurants.Services/dump/db/init-restaurants.sql`
- **auth**: Auth.Services database → `Auth.Services/dump/db/init-auth.sql`
- **order**: Order.Services database → `Order.Services/dump/db/init.sql`

## What It Does

1. **Checks database connectivity** - Ensures Docker containers are running
2. **Dumps schema + data** - Creates clean SQL files with `--clean` and `--if-exists` flags
3. **Filters out psql meta-commands** - Removes `\restrict` and `\unrestrict` commands that make files unimportable
4. **Verifies output** - Ensures dump files are created and have content

## Requirements

- Docker containers must be running (`docker-compose up`)
- PostgreSQL must be accessible on localhost:5432
- All service databases must exist

## After Dumping

Remember to commit the updated dump files:

```bash
git add */dump/db/*.sql
git commit -m "Update database dumps"
```

This ensures other developers can get the latest data when they pull the repository.