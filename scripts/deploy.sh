#!/bin/bash
# Orange Car Rental - Easy Deployment Script
#
# Usage:
#   ./scripts/deploy.sh [options]
#
# Options:
#   --tag TAG       Docker image tag (default: latest)
#   --env ENV       Environment: staging|production (default: staging)
#   --build         Build images locally instead of pulling from GHCR
#   --down          Stop and remove containers
#   --logs          Show container logs
#   --status        Show container status
#   --help          Show this help message

set -e

# Default values
IMAGE_TAG="${IMAGE_TAG:-latest}"
ENVIRONMENT="${ENVIRONMENT:-staging}"
GHCR_REPO="${GHCR_REPO:-smartsolutionslab/orange-car-rental-solution}"
DB_PASSWORD="${DB_PASSWORD:-postgres}"
BUILD_LOCAL=false
COMPOSE_FILE="docker-compose.deploy.yml"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

show_help() {
    head -20 "$0" | tail -17
    exit 0
}

show_status() {
    print_info "Container Status:"
    docker compose -f $COMPOSE_FILE ps
    exit 0
}

show_logs() {
    docker compose -f $COMPOSE_FILE logs -f
    exit 0
}

stop_containers() {
    print_info "Stopping containers..."
    docker compose -f $COMPOSE_FILE down
    print_info "Containers stopped."
    exit 0
}

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --tag)
            IMAGE_TAG="$2"
            shift 2
            ;;
        --env)
            ENVIRONMENT="$2"
            shift 2
            ;;
        --build)
            BUILD_LOCAL=true
            COMPOSE_FILE="src/docker-compose.yml"
            shift
            ;;
        --down)
            stop_containers
            ;;
        --logs)
            show_logs
            ;;
        --status)
            show_status
            ;;
        --help)
            show_help
            ;;
        *)
            print_error "Unknown option: $1"
            show_help
            ;;
    esac
done

# Check for docker
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed. Please install Docker first."
    exit 1
fi

print_info "========================================"
print_info "  Orange Car Rental Deployment"
print_info "========================================"
print_info "Environment: $ENVIRONMENT"
print_info "Image Tag:   $IMAGE_TAG"
print_info "Build Local: $BUILD_LOCAL"
print_info "========================================"

if [ "$BUILD_LOCAL" = true ]; then
    print_info "Building images locally..."
    cd src
    docker compose build
    cd ..
else
    print_info "Pulling images from GHCR..."
    export GHCR_REPO
    export IMAGE_TAG
    export DB_PASSWORD

    # Login to GHCR if token is available
    if [ -n "$GITHUB_TOKEN" ]; then
        echo "$GITHUB_TOKEN" | docker login ghcr.io -u "$GITHUB_USER" --password-stdin
    fi

    docker compose -f $COMPOSE_FILE pull
fi

print_info "Starting containers..."
export GHCR_REPO
export IMAGE_TAG
export DB_PASSWORD

if [ "$ENVIRONMENT" = "production" ]; then
    docker compose -f $COMPOSE_FILE -f src/docker-compose.prod.yml up -d
else
    docker compose -f $COMPOSE_FILE up -d
fi

print_info "Waiting for services to be healthy..."
sleep 10

print_info "Deployment complete!"
print_info ""
print_info "Services available at:"
print_info "  - Public Portal:      http://localhost:4301"
print_info "  - Call Center Portal: http://localhost:4302"
print_info "  - API Gateway:        http://localhost:5002"
print_info ""
print_info "Commands:"
print_info "  View logs:   $0 --logs"
print_info "  View status: $0 --status"
print_info "  Stop:        $0 --down"
