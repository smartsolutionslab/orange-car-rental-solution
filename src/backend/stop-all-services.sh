#!/bin/bash

# Orange Car Rental - Stop All Backend Services

echo "🛑 Stopping all Orange Car Rental services..."
echo ""

if [ ! -d "logs" ]; then
    echo "✅ No services are currently running (no logs directory found)."
    exit 0
fi

# Read PIDs from files and kill processes
count=0
for pidfile in logs/*.pid; do
    if [ -f "$pidfile" ]; then
        pid=$(cat "$pidfile")
        name=$(basename "$pidfile" .pid)

        if kill -0 "$pid" 2>/dev/null; then
            echo "   • Stopping $name (PID: $pid)"
            kill "$pid" 2>/dev/null || kill -9 "$pid" 2>/dev/null
            ((count++))
        fi

        rm "$pidfile"
    fi
done

# Also kill any dotnet processes running OrangeCarRental
pkill -f "OrangeCarRental" 2>/dev/null && ((count++)) || true

if [ $count -eq 0 ]; then
    echo "✅ No services were running."
else
    echo ""
    echo "✅ Stopped $count service(s)!"
fi

echo ""
