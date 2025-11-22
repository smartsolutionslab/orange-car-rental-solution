#!/bin/bash
# Pre-commit validation script
# Run this before committing to ensure code quality

set -e

echo "======================================"
echo "Pre-Commit Validation"
echo "======================================"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

FAILED=0

# Check 1: Run linting
echo -e "${YELLOW}[1/4] Running linting...${NC}"
cd src/frontend/apps/public-portal
if npm run lint --silent; then
    echo -e "${GREEN}✓ Public portal linting passed${NC}"
else
    echo -e "${RED}✗ Public portal linting failed${NC}"
    FAILED=1
fi

cd ../call-center-portal
if npm run lint --silent; then
    echo -e "${GREEN}✓ Call center portal linting passed${NC}"
else
    echo -e "${RED}✗ Call center portal linting failed${NC}"
    FAILED=1
fi

cd ../../../..

echo ""

# Check 2: Run unit tests
echo -e "${YELLOW}[2/4] Running unit tests...${NC}"
cd src/frontend/apps/public-portal
if npm run test:ci --silent; then
    echo -e "${GREEN}✓ Public portal unit tests passed${NC}"
else
    echo -e "${RED}✗ Public portal unit tests failed${NC}"
    FAILED=1
fi

cd ../call-center-portal
if npm run test:ci --silent; then
    echo -e "${GREEN}✓ Call center portal unit tests passed${NC}"
else
    echo -e "${RED}✗ Call center portal unit tests failed${NC}"
    FAILED=1
fi

cd ../../../..

echo ""

# Check 3: Check for console.log statements
echo -e "${YELLOW}[3/4] Checking for console.log statements...${NC}"
if git diff --cached --name-only --diff-filter=ACM | grep -E '\.(ts|js)$' | xargs grep -n 'console\.log' 2>/dev/null; then
    echo -e "${YELLOW}⚠ Warning: console.log statements found in staged files${NC}"
    echo -e "${YELLOW}  Consider removing them before committing${NC}"
else
    echo -e "${GREEN}✓ No console.log statements found${NC}"
fi

echo ""

# Check 4: Check for TODO comments
echo -e "${YELLOW}[4/4] Checking for TODO comments...${NC}"
TODO_COUNT=$(git diff --cached --name-only --diff-filter=ACM | grep -E '\.(ts|js)$' | xargs grep -c 'TODO\|FIXME' 2>/dev/null || true)
if [ "$TODO_COUNT" -gt 0 ]; then
    echo -e "${YELLOW}⚠ Found $TODO_COUNT TODO/FIXME comments in staged files${NC}"
    git diff --cached --name-only --diff-filter=ACM | grep -E '\.(ts|js)$' | xargs grep -n 'TODO\|FIXME' 2>/dev/null || true
else
    echo -e "${GREEN}✓ No TODO/FIXME comments found${NC}"
fi

echo ""
echo "======================================"

if [ $FAILED -eq 1 ]; then
    echo -e "${RED}Pre-commit validation FAILED${NC}"
    echo -e "${RED}Please fix the issues above before committing${NC}"
    exit 1
else
    echo -e "${GREEN}Pre-commit validation PASSED${NC}"
    echo -e "${GREEN}Ready to commit!${NC}"
    exit 0
fi
