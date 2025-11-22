#!/bin/bash
# ============================================================================
# Orange Car Rental - Database Seed Data Script (Bash)
# ============================================================================
# This script applies seed data to the PostgreSQL database
# Run after migrations have been applied
# ============================================================================

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Parameters with defaults
DB_HOST="${1:-localhost}"
DB_PORT="${2:-5432}"
DB_USER="${3:-orange_user}"
DB_PASSWORD="${4:-orange_dev_password}"
DB_NAME="${5:-orange_rental}"

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}Orange Car Rental - Seed Data${NC}"
echo -e "${CYAN}========================================${NC}"
echo -e "${YELLOW}Host:     $DB_HOST:$DB_PORT${NC}"
echo -e "${YELLOW}Database: $DB_NAME${NC}"
echo -e "${YELLOW}User:     $DB_USER${NC}"
echo ""

SEED_FILE="scripts/db/seed-test-data.sql"

if [ ! -f "$SEED_FILE" ]; then
    echo -e "${RED}ERROR: Seed file not found: $SEED_FILE${NC}"
    exit 1
fi

echo -e "${CYAN}Checking if PostgreSQL client (psql) is installed...${NC}"

if command -v psql &> /dev/null; then
    PSQL_VERSION=$(psql --version)
    echo -e "${GREEN}Found: $PSQL_VERSION${NC}"
else
    echo -e "${RED}ERROR: psql not found. Please install PostgreSQL client tools.${NC}"
    echo ""
    echo -e "${YELLOW}Alternative: Run seed data inside Docker container:${NC}"
    echo -e "${YELLOW}  docker exec -i orange-rental-db psql -U $DB_USER -d $DB_NAME < $SEED_FILE${NC}"
    exit 1
fi

echo ""
echo -e "${CYAN}Applying seed data...${NC}"

export PGPASSWORD="$DB_PASSWORD"

if psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f "$SEED_FILE"; then
    echo ""
    echo -e "${CYAN}========================================${NC}"
    echo -e "${GREEN}SUCCESS: Seed data applied successfully!${NC}"
    echo -e "${CYAN}========================================${NC}"
    exit 0
else
    echo ""
    echo -e "${CYAN}========================================${NC}"
    echo -e "${RED}FAILED: Error applying seed data${NC}"
    echo -e "${CYAN}========================================${NC}"
    exit 1
fi

unset PGPASSWORD
