#!/bin/bash
# Bundle Analysis Script
# Analyzes the bundle size and composition of frontend applications

echo "======================================"
echo "Bundle Size Analysis"
echo "======================================"
echo ""

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

# Analyze Public Portal
echo -e "${CYAN}Analyzing Public Portal...${NC}"
cd src/frontend/apps/public-portal

# Build production bundle
npm run build:production -- --stats-json

if [ -f "dist/public-portal/stats.json" ]; then
    echo -e "${GREEN}✓ Build complete${NC}"

    # Calculate bundle size
    BUNDLE_SIZE=$(du -sh dist/public-portal | cut -f1)
    echo -e "${YELLOW}Total bundle size: $BUNDLE_SIZE${NC}"

    # Show main bundle files
    echo ""
    echo "Main bundle files:"
    ls -lh dist/public-portal/*.js 2>/dev/null | awk '{print $9, $5}' || echo "No JS files found"

    # If webpack-bundle-analyzer is available, generate report
    if command -v npx &> /dev/null; then
        echo ""
        echo -e "${CYAN}Generating interactive bundle report...${NC}"
        npx webpack-bundle-analyzer dist/public-portal/stats.json dist/public-portal --mode static --report bundle-report-public.html --no-open
        echo -e "${GREEN}✓ Report saved to: bundle-report-public.html${NC}"
    fi
else
    echo -e "${RED}✗ Build failed or stats.json not generated${NC}"
fi

cd ../../../..

echo ""

# Analyze Call Center Portal
echo -e "${CYAN}Analyzing Call Center Portal...${NC}"
cd src/frontend/apps/call-center-portal

# Build production bundle
npm run build:production -- --stats-json

if [ -f "dist/call-center-portal/stats.json" ]; then
    echo -e "${GREEN}✓ Build complete${NC}"

    # Calculate bundle size
    BUNDLE_SIZE=$(du -sh dist/call-center-portal | cut -f1)
    echo -e "${YELLOW}Total bundle size: $BUNDLE_SIZE${NC}"

    # Show main bundle files
    echo ""
    echo "Main bundle files:"
    ls -lh dist/call-center-portal/*.js 2>/dev/null | awk '{print $9, $5}' || echo "No JS files found"

    # If webpack-bundle-analyzer is available, generate report
    if command -v npx &> /dev/null; then
        echo ""
        echo -e "${CYAN}Generating interactive bundle report...${NC}"
        npx webpack-bundle-analyzer dist/call-center-portal/stats.json dist/call-center-portal --mode static --report bundle-report-callcenter.html --no-open
        echo -e "${GREEN}✓ Report saved to: bundle-report-callcenter.html${NC}"
    fi
else
    echo -e "${RED}✗ Build failed or stats.json not generated${NC}"
fi

cd ../../../..

echo ""
echo "======================================"
echo -e "${GREEN}Bundle analysis complete${NC}"
echo "======================================"
