#!/bin/bash

# Orange Car Rental - Start All Backend Services
# Run this script from the backend directory

set -e

echo "🚀 Starting Orange Car Rental Backend Services..."
echo ""

# Check if running from correct directory
if [ ! -d "ApiGateway" ]; then
    echo "❌ Error: Please run this script from the backend directory"
    echo "   cd src/backend"
    exit 1
fi

echo "📋 Port Assignments:"
echo "   API Gateway:     http://localhost:5002"
echo "   Fleet API:       http://localhost:5000"
echo "   Reservations:    http://localhost:5001"
echo "   Customers:       http://localhost:5003"
echo "   Payments:        http://localhost:5004"
echo "   Notifications:   http://localhost:5005"
echo "   Locations:       http://localhost:5006"
echo ""

echo "🏗️  Building all projects first..."
dotnet build --nologo --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Build failed! Please fix errors and try again."
    exit 1
fi

echo "✅ Build successful!"
echo ""

echo "🚀 Launching services..."
echo ""

# Create logs directory
mkdir -p logs

# Function to start a service in background
start_service() {
    local name=$1
    local project=$2
    local port=$3

    echo "▶️  Starting $name on port $port..."

    dotnet run --project "$project" --no-launch-profile -- --urls "http://localhost:$port" \
        > "logs/$name.log" 2>&1 &

    echo $! > "logs/$name.pid"
    sleep 1
}

# Start services
start_service "API-Gateway" "ApiGateway/OrangeCarRental.ApiGateway" "5002"
start_service "Fleet-API" "Services/Fleet/OrangeCarRental.Fleet.Api" "5000"
start_service "Reservations-API" "Services/Reservations/OrangeCarRental.Reservations.Api" "5001"
start_service "Customers-API" "Services/Customers/OrangeCarRental.Customers.Api" "5003"
start_service "Payments-API" "Services/Payments/OrangeCarRental.Payments.Api" "5004"
start_service "Notifications-API" "Services/Notifications/OrangeCarRental.Notifications.Api" "5005"
start_service "Locations-API" "Services/Location/OrangeCarRental.Location.Api" "5006"

echo ""
echo "⏳ Waiting for services to start (45 seconds)..."
echo ""
sleep 45

echo "🔍 Checking service health..."
echo ""

# Function to check health
check_health() {
    local name=$1
    local url=$2

    if curl -s -f "$url" > /dev/null 2>&1; then
        echo "   ✅ $name is healthy"
        return 0
    else
        echo "   ❌ $name is not responding"
        return 1
    fi
}

healthy_count=0
total_services=7

check_health "API Gateway" "http://localhost:5002/health" && ((healthy_count++)) || true
check_health "Fleet API" "http://localhost:5000/health" && ((healthy_count++)) || true
check_health "Reservations" "http://localhost:5001/health" && ((healthy_count++)) || true
check_health "Customers" "http://localhost:5003/health" && ((healthy_count++)) || true
check_health "Payments" "http://localhost:5004/health" && ((healthy_count++)) || true
check_health "Notifications" "http://localhost:5005/health" && ((healthy_count++)) || true
check_health "Locations" "http://localhost:5006/health" && ((healthy_count++)) || true

echo ""
echo "📊 Status: $healthy_count/$total_services services healthy"
echo ""

if [ $healthy_count -eq $total_services ]; then
    echo "🎉 All services started successfully!"
    echo ""
    echo "📖 Quick Links:"
    echo "   • API Gateway:        http://localhost:5002/health"
    echo "   • Fleet API Docs:     http://localhost:5000/scalar/v1"
    echo "   • Reservations Docs:  http://localhost:5001/scalar/v1"
    echo "   • Customers Docs:     http://localhost:5003/scalar/v1"
    echo "   • Payments Docs:      http://localhost:5004/scalar/v1"
    echo "   • Notifications Docs: http://localhost:5005/scalar/v1"
    echo "   • Locations Docs:     http://localhost:5006/scalar/v1"
    echo ""
    echo "💡 Next Steps:"
    echo "   1. Start frontend: cd ../frontend && npm start"
    echo "   2. Open browser:   http://localhost:4200"
    echo "   3. Test booking flow"
    echo ""
    echo "📝 View logs: tail -f logs/*.log"
    echo "🛑 To stop all services: ./stop-all-services.sh"
else
    echo "⚠️  Some services failed to start. Check logs:"
    echo "   tail -f logs/*.log"
    echo ""
    echo "   Try running ./stop-all-services.sh and then starting again."
fi

echo ""
