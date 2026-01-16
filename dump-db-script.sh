#!/bin/bash
# Unified Database Dump Script for All Services
# This script allows you to dump data from multiple services and clean the output for re-import
set -e # Exit on any error
# Configuration
POSTGRES_USER="postgres"
POSTGRES_PASSWORD="united_password"
POSTGRES_HOST="localhost"
POSTGRES_PORT="5432"
# Service configurations
declare -A SERVICES=(
  ["order"]="order_service_db:Order.Services/dump/db/init.sql"
  ["driver"]="driver_services:Driver.Services/dump/db/db_dump_clean.sql"
  ["auth"]="auth_services:Auth.Services/dump/db/init-auth.sql"
  ["restaurants"]="restaurants_services:Restaurants.Services/dump/db/init-restaurants.sql"
)
# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color
# Function to print colored output
print_status() {
  echo -e "${GREEN}✅ $1${NC}"
}
print_warning() {
  echo -e "${YELLOW}⚠️  $1${NC}"
}
print_error() {
  echo -e "${RED}❌ $1${NC}"
}
print_info() {
  echo -e "${BLUE}ℹ️  $1${NC}"
}
# Function to check if service is running
check_service_running() {
  local service_name=$1
  local db_name=$2
  print_info "Checking if $service_name database is accessible..."
  if PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -p "$POSTGRES_PORT" -U "$POSTGRES_USER" -d "$db_name" -c "SELECT 1;" >/dev/null 2>&1; then
    print_status "$service_name database is accessible"
    return 0
  else
    print_error "$service_name database is not accessible. Make sure Docker containers are running."
    return 1
  fi
}
# Function to dump a single service
dump_service() {
  local service_key=$1
  local db_name=$2
  local output_file=$3
  print_info "Starting dump for $service_key service..."
  # Create directory if it doesn't exist
  mkdir -p "$(dirname "$output_file")"
  # Create clean SQL dump (schema + data)
  print_info "Creating clean schema + data dump..."
  # Run pg_dump and capture output
  local temp_file="${output_file}.tmp"
  PGPASSWORD="$POSTGRES_PASSWORD" pg_dump \
    -h "$POSTGRES_HOST" \
    -p "$POSTGRES_PORT" \
    -U "$POSTGRES_USER" \
    -d "$db_name" \
    --no-owner \
    --no-privileges \
    --clean \
    --if-exists \
    --format=plain >"$temp_file" 2>&1
  # Filter out restrict/unrestrict lines AND Entity Framework SET statements
  grep -v '\\restrict\|\\unrestrict' "$temp_file" |
    grep -v '^SET statement_timeout = 0;$' |
    grep -v '^SET lock_timeout = 0;$' |
    grep -v '^SET idle_in_transaction_session_timeout = 0;$' |
    grep -v '^SET transaction_timeout = 0;$' |
    grep -v '^SET client_encoding = '\''UTF8'\'';$' |
    grep -v '^SET standard_conforming_strings = on;$' |
    grep -v '^SELECT pg_catalog\.set_config('\''search_path'\'', '\'''\'', false);$' |
    grep -v '^SET check_function_bodies = false;$' |
    grep -v '^SET xmloption = content;$' |
    grep -v '^SET client_min_messages = warning;$' |
    grep -v '^SET row_security = off;$' >"$output_file"

  rm -f "$temp_file"
  # Verify the file was created and has content
  if [ -s "$output_file" ]; then
    print_status "Clean SQL dump created: $output_file"
    return 0
  else
    print_error "Dump file is empty for $service_key"
    return 1
  fi
}
# Function to show menu and get user selection
show_menu() {
  echo
  echo "========================================"
  echo "   Unified Database Dump Script"
  echo "========================================"
  echo
  echo "Available services:"
  local i=1
  for key in "${!SERVICES[@]}"; do
    IFS=':' read -r db_name file_path <<<"${SERVICES[$key]}"
    echo "  $i) $key (database: $db_name)"
    ((i++))
  done
  echo "  $i) All services"
  echo "  0) Exit"
  echo
}
# Function to get user selection
get_selection() {
  local max_options=${#SERVICES[@]}
  local all_option=$((max_options + 1))
  while true; do
    read -p "Enter your choice (0-$all_option): " choice
    echo
    if [[ "$choice" =~ ^[0-9]+$ ]]; then
      if [ "$choice" -eq 0 ]; then
        echo "Exiting..."
        exit 0
      elif [ "$choice" -eq "$all_option" ]; then
        # Select all services
        SELECTED_SERVICES=("${!SERVICES[@]}")
        break
      elif [ "$choice" -ge 1 ] && [ "$choice" -le "$max_options" ]; then
        # Select specific service
        local i=1
        for key in "${!SERVICES[@]}"; do
          if [ "$i" -eq "$choice" ]; then
            SELECTED_SERVICES=("$key")
            break
          fi
          ((i++))
        done
        break
      fi
    fi
    print_error "Invalid choice. Please enter a number between 0 and $all_option."
  done
}
# Function to process services
process_services() {
  local services_to_process=("$@")
  local success_count=0
  local total_count=${#services_to_process[@]}
  print_info "Processing ${#services_to_process[@]} service(s): ${services_to_process[*]}"
  for service_key in "${services_to_process[@]}"; do
    if [[ -v SERVICES[$service_key] ]]; then
      IFS=':' read -r db_name file_path <<<"${SERVICES[$service_key]}"
      if check_service_running "$service_key" "$db_name"; then
        if dump_service "$service_key" "$db_name" "$file_path"; then
          success_count=$((success_count + 1))
        fi
      fi
    else
      print_error "Unknown service: $service_key"
    fi
    echo
  done
  # Summary
  echo "========================================"
  if [ "$success_count" -eq "$total_count" ]; then
    print_status "All dumps completed successfully! ($success_count/$total_count)"
  else
    print_warning "Some dumps failed. ($success_count/$total_count successful)"
  fi
  echo
  print_info "Remember to commit the updated dump files:"
  echo "  git add */dump/db/*.sql"
  echo "  git commit -m \"Update database dumps\""
  echo "========================================"
}
# Main script
main() {
  # Check if Docker containers are running
  print_info "Checking database connectivity..."
  if ! PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -p "$POSTGRES_PORT" -U "$POSTGRES_USER" -d "postgres" -c "SELECT 1;" >/dev/null 2>&1; then
    print_error "Cannot connect to PostgreSQL. Make sure Docker containers are running with 'docker-compose up'."
    exit 1
  fi
  # Check command line arguments
  if [ $# -gt 0 ]; then
    # Process command line arguments
    process_services "$@"
    return
  fi
  # Interactive mode
  show_menu
  get_selection
  process_services "${SELECTED_SERVICES[@]}"
}
# Run main function
main "$@"

