# Orange Car Rental - Service Management Guide

Quick reference for starting, stopping, and managing all application services.

---

## 🎛️ Current Running Services

### Active Processes (Right Now)

```bash
# Check what's running
netstat -ano | findstr "LISTENING" | findstr ":4201 :5002 :1433 :8080"
```

**Your Current Stack:**
- Frontend (Angular): Port 4201
- API Gateway: Port 5002
- SQL Server: Port 1433
- Keycloak: Port 8080
- Aspire Dashboard: Port 17161

---

## 🚀 Starting Services

### Method 1: Start Everything with Aspire (Recommended)

**Single Command - Starts All Services:**
```bash
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run
```

**What This Starts:**
- ✅ SQL Server container
- ✅ Keycloak container
- ✅ All 4 backend APIs (Customers, Fleet, Reservations, Pricing)
- ✅ API Gateway
- ✅ Frontend apps (Public Portal, Call Center Portal)
- ✅ Aspire Dashboard for monitoring

**Output:**
```
info: Aspire.Hosting.DistributedApplication[0]
      Now listening on: https://localhost:17161
      Login to the dashboard at https://localhost:17161/login?t={token}
```

**Access URLs (Aspire manages these):**
- Dashboard: URL shown in console
- Services: Auto-configured and routed

### Method 2: Start Services Individually (Advanced)

**If you need granular control:**

```bash
# 1. Start SQL Server (Docker)
docker-compose -f docker-compose.sqlserver.yml up -d

# 2. Start Keycloak (Docker)
docker-compose -f docker-compose.keycloak.yml up -d

# 3. Start Backend APIs individually
cd src/backend/Services/Fleet/OrangeCarRental.Fleet.Api
dotnet run

# (Repeat for Customers, Reservations, Pricing)

# 4. Start API Gateway
cd src/backend/ApiGateway
dotnet run

# 5. Start Frontend
cd src/frontend/apps/public-portal
npm start
```

**Note:** Method 1 (Aspire) is much simpler and handles all orchestration automatically.

---

## 🛑 Stopping Services

### Stop Aspire (Recommended Method)

**In the terminal running Aspire:**
```bash
# Press Ctrl+C
# Aspire will gracefully shut down all services
```

**What Aspire Stops:**
- All backend APIs
- API Gateway
- Frontend apps
- Database container (if started by Aspire)
- Keycloak container

### Stop Individual Services

**Find and Kill Processes:**
```bash
# List processes on specific ports
netstat -ano | findstr ":4201"  # Frontend
netstat -ano | findstr ":5002"  # API Gateway
netstat -ano | findstr ":1433"  # SQL Server

# Kill a process by PID (Windows)
taskkill /PID <process_id> /F

# Example:
taskkill /PID 19268 /F  # Kills frontend on port 4201
```

**Stop Docker Containers:**
```bash
# List running containers
docker ps

# Stop specific container
docker stop orange-rental-sqlserver
docker stop orange-rental-keycloak

# Stop all Orange Car Rental containers
docker ps --filter "name=orange-rental" -q | ForEach-Object { docker stop $_ }

# Or using docker-compose
docker-compose -f docker-compose.sqlserver.yml down
```

### Complete Cleanup (Stop Everything)

**PowerShell Script:**
```powershell
# Stop all Orange Car Rental related processes

# 1. Stop Aspire (if running)
Get-Process | Where-Object {$_.ProcessName -like "*OrangeCarRental*"} | Stop-Process -Force

# 2. Stop frontend
Get-Process | Where-Object {$_.CommandLine -like "*public-portal*"} | Stop-Process -Force

# 3. Stop Docker containers
docker ps --filter "name=orange" -q | ForEach-Object { docker stop $_ }

Write-Host "All Orange Car Rental services stopped" -ForegroundColor Green
```

**Save as:** `scripts/stop-all.ps1`

**Run:**
```bash
powershell -ExecutionPolicy Bypass -File scripts/stop-all.ps1
```

---

## 🔄 Restarting Services

### Restart Everything (Aspire)

```bash
# In Aspire terminal:
# 1. Press Ctrl+C to stop
# 2. Wait for graceful shutdown
# 3. Run again:
dotnet run
```

### Restart Individual Service

**Backend API:**
```bash
# Stop the service (Ctrl+C in its terminal)
# OR kill process:
taskkill /PID <pid> /F

# Restart
cd src/backend/Services/Fleet/OrangeCarRental.Fleet.Api
dotnet run
```

**Frontend:**
```bash
# Stop (Ctrl+C)
# Restart
cd src/frontend/apps/public-portal
npm start
```

**Docker Container:**
```bash
docker restart orange-rental-sqlserver
docker restart orange-rental-keycloak
```

---

## 🔍 Monitoring Running Services

### Check Service Health

**API Gateway Health:**
```bash
curl http://localhost:5002/health
```

**Expected Response:**
```json
{"service":"API Gateway","status":"Healthy","timestamp":"..."}
```

**Individual Service Health:**
```bash
# Via API Gateway
curl http://localhost:5002/api/vehicles/health
curl http://localhost:5002/api/customers/health
curl http://localhost:5002/api/reservations/health
```

### View Logs

**Aspire Dashboard (Best Option):**
```
1. Open https://localhost:17161
2. Navigate to "Logs" tab
3. Filter by service, log level, or search text
```

**Console Logs:**
- Aspire shows aggregated logs from all services
- Individual services log to their console if run separately

**Docker Logs:**
```bash
docker logs orange-rental-sqlserver
docker logs orange-rental-keycloak

# Follow logs in real-time
docker logs -f orange-rental-sqlserver
```

### Check Ports in Use

**Windows:**
```bash
# All listening ports
netstat -ano | findstr LISTENING

# Specific port
netstat -ano | findstr :5002

# See which process owns a port
# Look at the last column (PID)
# Then: tasklist /FI "PID eq <pid>"
```

**Check Orange Car Rental Ports:**
```bash
# Create a script: scripts/check-ports.ps1
$ports = @(4200, 4201, 5002, 8080, 1433, 17161)
foreach ($port in $ports) {
    $result = netstat -ano | findstr ":$port.*LISTENING"
    if ($result) {
        Write-Host "Port $port is in use" -ForegroundColor Green
    } else {
        Write-Host "Port $port is available" -ForegroundColor Yellow
    }
}
```

---

## 🐛 Troubleshooting

### Port Already in Use

**Problem:**
```
Error: listen EADDRINUSE: address already in use :::4201
```

**Solution:**
```bash
# 1. Find process using the port
netstat -ano | findstr :4201

# 2. Kill the process
taskkill /PID <pid> /F

# 3. Restart your service
```

### Service Won't Start

**Checklist:**
1. ✅ Docker Desktop is running
2. ✅ Required ports are available
3. ✅ Database is accessible
4. ✅ Environment variables are set
5. ✅ Previous instance is stopped

**Verify Docker:**
```bash
docker ps
docker version
```

**Verify Database Connection:**
```bash
# Test SQL Server connection
docker exec orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C -Q "SELECT @@VERSION"
```

### Aspire Dashboard Not Accessible

**Problem:** Can't access https://localhost:17161

**Solutions:**
```bash
# 1. Check if Aspire is running
Get-Process | Where-Object {$_.ProcessName -like "*OrangeCarRental.AppHost*"}

# 2. Check console output for actual URL (might be different port)

# 3. Try the login URL with token from console

# 4. Restart Aspire
# Ctrl+C, then dotnet run again
```

### Frontend Not Loading

**Problem:** http://localhost:4201 doesn't respond

**Solutions:**
```bash
# 1. Check if process is running
netstat -ano | findstr :4201

# 2. Check for build errors
cd src/frontend/apps/public-portal
npm start
# Look for compilation errors

# 3. Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install
npm start
```

---

## 🔐 Database Management

### Connect to Database

**SQL Server Management Studio:**
```
Server: localhost,1433
Authentication: SQL Server Authentication
User: sa
Password: YourStrong@Passw0rd
```

**Command Line:**
```bash
# Via Docker
docker exec -it orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C

# List databases
SELECT name FROM sys.databases;
GO
```

### Backup Database

```bash
# Backup all databases
docker exec orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C -Q "BACKUP DATABASE [OrangeCarRental_Fleet] TO DISK = '/var/opt/mssql/backup/fleet.bak'"

# Copy backup to host
docker cp orange-rental-sqlserver:/var/opt/mssql/backup/fleet.bak ./backups/
```

### Reset Database

**Warning:** This deletes all data!

```bash
# Stop all services first
# Then:

# 1. Drop and recreate databases
docker exec orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C -Q "DROP DATABASE OrangeCarRental_Fleet; DROP DATABASE OrangeCarRental_Customers; DROP DATABASE OrangeCarRental_Reservations; DROP DATABASE OrangeCarRental_Pricing;"

# 2. Run migrations again
cd src/backend
dotnet ef database update --project Services/Fleet/OrangeCarRental.Fleet.Api
# (Repeat for all services)

# 3. Re-apply seed data
Get-Content scripts/db/seed-test-data-sqlserver.sql | docker exec -i orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

---

## 📋 Common Commands Cheat Sheet

```bash
# ===== STARTING =====
# Start everything (recommended)
cd src/backend/AppHost/OrangeCarRental.AppHost && dotnet run

# Start just database
docker-compose -f docker-compose.sqlserver.yml up -d

# ===== STOPPING =====
# Stop Aspire: Ctrl+C in terminal

# Stop database
docker stop orange-rental-sqlserver

# Kill process by port
netstat -ano | findstr :5002  # Get PID
taskkill /PID <pid> /F         # Kill it

# ===== MONITORING =====
# Check health
curl http://localhost:5002/health

# View logs
# → Open Aspire Dashboard

# Check ports
netstat -ano | findstr "4201 5002 1433"

# ===== DATABASE =====
# Connect
docker exec -it orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C

# Run seed data
Get-Content scripts/db/seed-test-data-sqlserver.sql | docker exec -i orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

---

## 🎯 Recommended Workflow

### Daily Development Workflow

```bash
# Morning: Start everything
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run

# Open Aspire Dashboard (URL from console)
# Open Application (http://localhost:4201)

# During development:
# - Make code changes
# - Aspire auto-reloads services
# - Check logs in Aspire Dashboard

# Evening: Stop everything
# Ctrl+C in Aspire terminal
```

### Clean Start (After System Restart)

```bash
# 1. Ensure Docker Desktop is running
docker ps

# 2. Start Aspire
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run

# 3. Wait for all services to be healthy
# Check Aspire Dashboard

# 4. Start developing
```

### Before Committing Code

```bash
# 1. Run tests
cd src/backend
dotnet test

# 2. Verify all services still work
curl http://localhost:5002/health

# 3. Check Aspire Dashboard for errors

# 4. Stop services (optional)
# Ctrl+C
```

---

## 🚨 Emergency Recovery

### Everything is Broken

```bash
# 1. Stop everything
taskkill /F /IM dotnet.exe
docker stop $(docker ps -q)

# 2. Clean Docker
docker system prune -f

# 3. Clean .NET build artifacts
cd src/backend
dotnet clean

# 4. Restart Docker Desktop

# 5. Start fresh
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run
```

### Database is Corrupted

```bash
# Nuclear option - complete reset
docker-compose -f docker-compose.sqlserver.yml down -v
docker-compose -f docker-compose.sqlserver.yml up -d

# Wait for SQL Server to start
Start-Sleep -Seconds 10

# Run migrations
cd src/backend
dotnet ef database update --project Services/Fleet/OrangeCarRental.Fleet.Api
# (Repeat for all services)

# Re-apply seed data
Get-Content scripts/db/seed-test-data-sqlserver.sql | docker exec -i orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

---

## 📞 Quick Reference

| Task | Command |
|------|---------|
| Start All | `cd src/backend/AppHost/OrangeCarRental.AppHost && dotnet run` |
| Stop All | `Ctrl+C` in Aspire terminal |
| Check Health | `curl http://localhost:5002/health` |
| View Logs | Open Aspire Dashboard |
| Database Connect | `docker exec -it orange-rental-sqlserver /opt/mssql-tools18/bin/sqlcmd...` |
| Check Ports | `netstat -ano \| findstr "4201 5002"` |
| Kill Process | `taskkill /PID <pid> /F` |
| List Containers | `docker ps` |

---

**Pro Tip:** Bookmark the Aspire Dashboard - it's your central monitoring hub for all services!

**Remember:** Always use `Ctrl+C` to stop Aspire gracefully. Don't kill the process forcefully unless necessary.
