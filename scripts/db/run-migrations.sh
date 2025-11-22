#!/bin/bash
# ============================================================================
# Orange Car Rental - Database Migration Script (Bash)
# ============================================================================
# This script runs EF Core migrations for all microservices
# Run from project root directory
# ============================================================================

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Parameters
PROVIDER="${1:-PostgreSQL}"
CREATE_MIGRATION="${2:-false}"
MIGRATION_NAME="${3:-NewMigration}"

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}Orange Car Rental - Database Migrations${NC}"
echo -e "${CYAN}========================================${NC}"
echo -e "${YELLOW}Provider: $PROVIDER${NC}"
echo ""

# Define services
declare -a services=(
    "Customers:src/backend/Services/Customers"
    "Fleet:src/backend/Services/Fleet"
    "Location:src/backend/Services/Location"
    "Reservations:src/backend/Services/Reservations"
    "Pricing:src/backend/Services/Pricing"
    "Payments:src/backend/Services/Payments"
    "Notifications:src/backend/Services/Notifications"
)

SUCCESS_COUNT=0
FAILURE_COUNT=0
FAILED_SERVICES=()

for service_info in "${services[@]}"; do
    IFS=':' read -r service_name service_path <<< "$service_info"
    api_path="$service_path/OrangeCarRental.$service_name.Api"

    echo -e "${YELLOW}Processing: $service_name Service...${NC}"

    if [ ! -d "$api_path" ]; then
        echo -e "${RED}  ERROR: Path not found: $api_path${NC}"
        ((FAILURE_COUNT++))
        FAILED_SERVICES+=("$service_name")
        continue
    fi

    pushd "$api_path" > /dev/null 2>&1

    if [ "$CREATE_MIGRATION" = "true" ]; then
        echo -e "${CYAN}  Creating new migration: $MIGRATION_NAME${NC}"
        if dotnet ef migrations add "$MIGRATION_NAME" --verbose; then
            echo -e "${GREEN}  SUCCESS: $service_name${NC}"
            ((SUCCESS_COUNT++))
        else
            echo -e "${RED}  FAILED: $service_name${NC}"
            ((FAILURE_COUNT++))
            FAILED_SERVICES+=("$service_name")
        fi
    else
        echo -e "${CYAN}  Applying migrations...${NC}"
        if dotnet ef database update --verbose; then
            echo -e "${GREEN}  SUCCESS: $service_name${NC}"
            ((SUCCESS_COUNT++))
        else
            echo -e "${RED}  FAILED: $service_name${NC}"
            ((FAILURE_COUNT++))
            FAILED_SERVICES+=("$service_name")
        fi
    fi

    popd > /dev/null 2>&1
    echo ""
done

# Summary
echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}Migration Summary${NC}"
echo -e "${CYAN}========================================${NC}"
echo -e "${GREEN}Successful: $SUCCESS_COUNT${NC}"
if [ $FAILURE_COUNT -gt 0 ]; then
    echo -e "${RED}Failed: $FAILURE_COUNT${NC}"
else
    echo -e "${GREEN}Failed: $FAILURE_COUNT${NC}"
fi

if [ ${#FAILED_SERVICES[@]} -gt 0 ]; then
    echo ""
    echo -e "${RED}Failed Services:${NC}"
    for failed in "${FAILED_SERVICES[@]}"; do
        echo -e "${RED}  - $failed${NC}"
    done
fi

echo -e "${CYAN}========================================${NC}"

# Exit with appropriate code
if [ $FAILURE_COUNT -gt 0 ]; then
    exit 1
else
    exit 0
fi
