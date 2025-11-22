#!/bin/bash
# Orange Car Rental - Local Development Setup Script
# This script automates the initial setup for local development

set -e

echo "====================================="
echo "Orange Car Rental - Local Setup"
echo "====================================="
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Check if Node.js is installed
echo -e "${YELLOW}Checking Node.js installation...${NC}"
if command -v node &> /dev/null; then
    NODE_VERSION=$(node --version)
    echo -e "${GREEN}✓ Node.js version: $NODE_VERSION${NC}"
else
    echo -e "${RED}✗ Node.js is not installed. Please install Node.js 20+ from https://nodejs.org/${NC}"
    exit 1
fi

# Check if .NET is installed
echo -e "${YELLOW}Checking .NET installation...${NC}"
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓ .NET version: $DOTNET_VERSION${NC}"
else
    echo -e "${RED}✗ .NET is not installed. Please install .NET 8.0+ from https://dotnet.microsoft.com/${NC}"
    exit 1
fi

echo ""
echo -e "${CYAN}=====================================${NC}"
echo -e "${CYAN}Step 1: Installing Root Dependencies${NC}"
echo -e "${CYAN}=====================================${NC}"

cd "$(dirname "$0")/.."
npm install

echo -e "${GREEN}✓ Root dependencies installed${NC}"

echo ""
echo -e "${CYAN}=====================================${NC}"
echo -e "${CYAN}Step 2: Installing Playwright Browsers${NC}"
echo -e "${CYAN}=====================================${NC}"

npx playwright install --with-deps

echo -e "${GREEN}✓ Playwright browsers installed${NC}"

echo ""
echo -e "${CYAN}=====================================${NC}"
echo -e "${CYAN}Step 3: Public Portal Setup${NC}"
echo -e "${CYAN}=====================================${NC}"

cd src/frontend/apps/public-portal
npm install

echo -e "${GREEN}✓ Public portal dependencies installed${NC}"

echo ""
echo -e "${CYAN}=====================================${NC}"
echo -e "${CYAN}Step 4: Call Center Portal Setup${NC}"
echo -e "${CYAN}=====================================${NC}"

cd ../call-center-portal
npm install

echo -e "${GREEN}✓ Call center portal dependencies installed${NC}"

echo ""
echo -e "${CYAN}=====================================${NC}"
echo -e "${CYAN}Step 5: Backend Setup${NC}"
echo -e "${CYAN}=====================================${NC}"

cd ../../../backend/Api
dotnet restore

echo -e "${GREEN}✓ Backend dependencies restored${NC}"

echo ""
echo -e "${CYAN}=====================================${NC}"
echo -e "${CYAN}Step 6: Environment Configuration${NC}"
echo -e "${CYAN}=====================================${NC}"

cd ../../..

if [ ! -f ".env" ]; then
    if [ -f ".env.example" ]; then
        cp .env.example .env
        echo -e "${GREEN}✓ Created .env file from template${NC}"
        echo -e "${YELLOW}⚠ Please edit .env with your test credentials${NC}"
    else
        echo -e "${YELLOW}⚠ .env.example not found, skipping .env creation${NC}"
    fi
else
    echo -e "${GREEN}✓ .env file already exists${NC}"
fi

echo ""
echo -e "${CYAN}=====================================${NC}"
echo -e "${GREEN}Setup Complete!${NC}"
echo -e "${CYAN}=====================================${NC}"
echo ""
echo -e "${YELLOW}Next Steps:${NC}"
echo "1. Edit .env file with your test credentials"
echo "2. Start the backend: cd src/backend/Api && dotnet run"
echo "3. Start public portal: cd src/frontend/apps/public-portal && npm start"
echo "4. Start call center: cd src/frontend/apps/call-center-portal && npm start"
echo "5. Run tests: npm test"
echo ""
echo -e "${YELLOW}For more information, see:${NC}"
echo "- QUICK-START-TESTING.md"
echo "- WHATS-NEXT.md"
echo "- README.md"
echo ""
echo -e "${GREEN}Happy coding! 🚀${NC}"
