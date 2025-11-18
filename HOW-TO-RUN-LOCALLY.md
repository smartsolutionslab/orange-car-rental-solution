# How to Run Orange Car Rental Locally

Complete guide for running all 7 microservices + API Gateway + 2 frontends locally.

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend Applications                     │
├─────────────────────────────────────────────────────────────┤
│  Public Portal          │  Call Center Portal               │
│  http://localhost:4200  │  http://localhost:4201            │
└──────────────┬──────────┴────────────────┬──────────────────┘
               │                            │
               └────────────┬───────────────┘
                            │
                            ▼
               ┌────────────────────────┐
               │     API Gateway        │
               │  http://localhost:5002 │
               │    (YARP Proxy)        │
               └────────────┬───────────┘
                            │
        ┌───────────────────┼───────────────────┐
        │                   │                   │
        ▼                   ▼                   ▼
┌───────────────┐  ┌───────────────┐  ┌───────────────┐
│   Fleet API   │  │ Reservations  │  │  Customers    │
│ localhost:5000│  │  localhost:   │  │ localhost:5003│
└───────┬───────┘  │     5001      │  └───────┬───────┘
        │          └───────┬───────┘          │
        │                  │                  │
        ▼                  ▼                  ▼
   fleet-db         reservations-db      customers-db
        │                  │                  │
        ▼                  ▼                  ▼
┌───────────────┐  ┌───────────────┐  ┌───────────────┐
│  Pricing API  │  │  Payments API │  │Notifications  │
│ localhost:5002│  │ localhost:5004│  │ localhost:5005│
└───────┬───────┘  └───────┬───────┘  └───────┬───────┘
        │                  │                  │
        ▼                  ▼                  ▼
   pricing-db          payments-db      notifications-db
        │                  │                  │
        └──────────────────┼──────────────────┘
                          │
                          ▼
                   ┌───────────────┐
                   │  Location API │
                   │ localhost:5006│
                   └───────┬───────┘
                           │
                           ▼
                      locations-db
```

---

## 📊 Service Port Assignments

| Service | HTTP Port | HTTPS Port | Database |
|---------|-----------|------------|----------|
| **API Gateway** | 5002 | 7002 | N/A |
| Fleet API | 5000 | 7000 | OrangeCarRental_Fleet |
| Reservations API | 5001 | 7001 | OrangeCarRental_Reservations |
| Pricing API | 5002 (via gateway) | 7002 | OrangeCarRental_Pricing |
| Customers API | 5003 | 7003 | OrangeCarRental_Customers |
| Payments API | 5004 | 7004 | OrangeCarRental_Payments |
| Notifications API | 5005 | 7005 | OrangeCarRental_Notifications |
| Location API | 5006 | 7006 | OrangeCarRental_Locations |
| **Public Portal** | 4200 | N/A | N/A |
| **Call Center Portal** | 4201 | N/A | N/A |

---

## 🚀 Quick Start (Recommended)

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+
- SQL Server (LocalDB or full instance)
- Git Bash or PowerShell

### Step 1: Start Databases (Auto-migrated)
All services auto-migrate their databases on startup in Development mode.
No manual database setup required! ✨

### Step 2: Start Backend Services

**Option A: PowerShell (Recommended)**
```powershell
# Navigate to backend directory
cd src/backend

# Start all services in separate windows
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "dotnet run --project ApiGateway/OrangeCarRental.ApiGateway"
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "dotnet run --project Services/Fleet/OrangeCarRental.Fleet.Api"
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "dotnet run --project Services/Reservations/OrangeCarRental.Reservations.Api"
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "dotnet run --project Services/Customers/OrangeCarRental.Customers.Api"
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "dotnet run --project Services/Pricing/OrangeCarRental.Pricing.Api"
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "dotnet run --project Services/Payments/OrangeCarRental.Payments.Api"
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "dotnet run --project Services/Notifications/OrangeCarRental.Notifications.Api"
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "dotnet run --project Services/Location/OrangeCarRental.Location.Api"
```

**Option B: Git Bash / Linux**
```bash
# Start in background with logs
dotnet run --project ApiGateway/OrangeCarRental.ApiGateway &
dotnet run --project Services/Fleet/OrangeCarRental.Fleet.Api &
dotnet run --project Services/Reservations/OrangeCarRental.Reservations.Api &
dotnet run --project Services/Customers/OrangeCarRental.Customers.Api &
dotnet run --project Services/Pricing/OrangeCarRental.Pricing.Api &
dotnet run --project Services/Payments/OrangeCarRental.Payments.Api &
dotnet run --project Services/Notifications/OrangeCarRental.Notifications.Api &
dotnet run --project Services/Location/OrangeCarRental.Location.Api &

# Wait for all to start (30-60 seconds)
sleep 60
```

### Step 3: Verify Backend Health
```bash
# Check API Gateway
curl http://localhost:5002/health

# Check individual services
curl http://localhost:5000/health  # Fleet
curl http://localhost:5001/health  # Reservations
curl http://localhost:5003/health  # Customers
curl http://localhost:5004/health  # Payments
curl http://localhost:5005/health  # Notifications
curl http://localhost:5006/health  # Locations
```

Expected response: `{"status":"healthy","service":"..."}`

### Step 4: Start Frontend Applications

```bash
# Navigate to frontend directory
cd ../../frontend

# Install dependencies (first time only)
npm install

# Start both portals (in separate terminals)
npm run start:public-portal    # Port 4200
npm run start:call-center      # Port 4201
```

**Or use Nx commands:**
```bash
npx nx serve public-portal     # Port 4200
npx nx serve call-center-portal # Port 4201
```

### Step 5: Access Applications

- **Public Portal:** http://localhost:4200
- **Call Center Portal:** http://localhost:4201
- **API Gateway:** http://localhost:5002
- **API Documentation (Scalar):**
  - Fleet: http://localhost:5000/scalar/v1
  - Reservations: http://localhost:5001/scalar/v1
  - Customers: http://localhost:5003/scalar/v1
  - Payments: http://localhost:5004/scalar/v1
  - Notifications: http://localhost:5005/scalar/v1
  - Locations: http://localhost:5006/scalar/v1

---

## 🧪 Testing the Complete System

### Test 1: Vehicle Search Flow

1. Open **Public Portal**: http://localhost:4200
2. Enter search criteria:
   - Pickup Date: Tomorrow
   - Return Date: +3 days
   - Location: MUC (Munich)
3. Click "Search Vehicles"
4. Verify vehicles are displayed

**Backend calls:**
```
Frontend → API Gateway (5002) → Fleet API (5000)
GET /api/vehicles?pickupDate=...&returnDate=...&locationCode=MUC
```

### Test 2: Complete Booking Flow

1. Search for vehicles (as above)
2. Select a vehicle
3. View price calculation
4. Enter customer details:
   - Name, Email, Phone
   - Driver's License
5. Confirm booking
6. Verify confirmation page

**Backend calls:**
```
1. GET /api/pricing/calculate
2. POST /api/customers
3. POST /api/reservations
4. POST /api/payments/process
5. POST /api/notifications/email
```

### Test 3: Call Center Flow

1. Open **Call Center Portal**: http://localhost:4201
2. Search for existing reservation
3. View reservation details
4. Modify or cancel booking

---

## 🐛 Troubleshooting

### Issue: Service won't start - "Port already in use"

**Solution:** Kill the process using the port
```powershell
# PowerShell
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -ti:5000 | xargs kill -9
```

### Issue: Database migration fails

**Solution:** Reset database
```bash
cd Services/Fleet/OrangeCarRental.Fleet.Infrastructure
dotnet ef database drop --force --startup-project ../OrangeCarRental.Fleet.Api
dotnet ef database update --startup-project ../OrangeCarRental.Fleet.Api
```

### Issue: Frontend can't connect to backend

**Check:**
1. API Gateway is running on port 5002
2. CORS is enabled (already configured)
3. Frontend config.json points to correct URL:
   ```json
   {
     "apiUrl": "http://localhost:5002"
   }
   ```

### Issue: 404 Not Found from API Gateway

**Check:**
1. Backend service is running
2. API Gateway routing configuration in `appsettings.json`
3. Service URL in API Gateway logs

**View logs:**
```
[12:00:00 INF] [APIGateway] Fleet API URL: http://localhost:5000
[12:00:00 INF] [APIGateway] Payments API URL: http://localhost:5004
```

---

## 📝 Development Tips

### Hot Reload
All services support .NET hot reload:
```bash
dotnet watch run --project Services/Fleet/OrangeCarRental.Fleet.Api
```

### Debug Multiple Services
Use Visual Studio or VS Code:
1. Create `launch.json` with multiple configurations
2. Use compound launch to start all at once
3. Set breakpoints across services

### View All Processes
```powershell
# PowerShell
Get-Process | Where-Object {$_.ProcessName -like "*OrangeCarRental*"}

# Kill all
Get-Process | Where-Object {$_.ProcessName -like "*OrangeCarRental*"} | Stop-Process -Force
```

---

## 🔍 Useful Commands

```bash
# Build all backend projects
dotnet build

# Run all backend tests
dotnet test

# Check code coverage
dotnet test /p:CollectCoverage=true

# Frontend build (production)
cd frontend
npm run build

# Frontend tests
npm test

# E2E tests (Playwright)
npx playwright test
```

---

## 📚 Next Steps

After verifying everything works locally:

1. ✅ **Write Frontend Tests** - Unit + E2E tests
2. 🚀 **Deploy to Azure** - Container Apps + SQL Databases
3. 🔐 **Add Authentication** - Keycloak/Auth0
4. 📊 **Set up Monitoring** - Application Insights
5. 🌍 **Add i18n** - English translations

---

**Created:** 2025-11-16
**Last Updated:** 2025-11-16
**Status:** All services operational ✅
