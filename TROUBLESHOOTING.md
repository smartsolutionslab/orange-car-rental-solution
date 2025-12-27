# Troubleshooting Guide

Quick solutions to common issues when developing with Orange Car Rental.

## Table of Contents

1. [Port Conflicts](#port-conflicts)
2. [Aspire Issues](#aspire-issues)
3. [Database Problems](#database-problems)
4. [Frontend Issues](#frontend-issues)
5. [API Errors](#api-errors)
6. [Docker Issues](#docker-issues)
7. [Build Failures](#build-failures)

---

## Port Conflicts

### Port Already in Use

**Symptom:**
```
Error: Address already in use: localhost:5002
```

**Solution:**
```bash
# Find process using the port
netstat -ano | findstr :5002

# Kill the process (Windows)
taskkill /PID <pid> /F

# Restart Aspire
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run
```

---

## Aspire Issues

### "no endpoints configured" Error

**Symptom:**
```
fail: Error handling TCP connection {"Service": {"name":"customers-api-http"},
"error": "no endpoints configured"}
```

**Solutions:**

1. **Wait for services to fully start** - Aspire services can take 30-60 seconds to initialize
2. **Check Aspire Dashboard** - Verify all services show "Running" status
3. **Restart affected service** from Aspire Dashboard

### Aspire Dashboard Not Accessible

**Solutions:**

1. Check console output for actual dashboard URL
2. Trust dev certificates: `dotnet dev-certs https --trust`
3. Restart Aspire

### Aspire Build Lock Error

**Symptom:**
```
error MSB3027: Could not copy "apphost.exe". The file is locked
```

**Solution:**
```bash
taskkill /PID <pid> /F
cd src/backend
dotnet clean
dotnet build
cd AppHost/OrangeCarRental.AppHost
dotnet run
```

---

## Database Problems

### Cannot Connect to SQL Server

**Solutions:**

1. **Check SQL Server container** in Aspire Dashboard
2. **View connection string** in Aspire Dashboard > Resources > Environment Variables
3. **Verify container is healthy** - look for green status

### Database Migrations Not Applied

**Symptom:**
```
SqlException: Invalid object name 'customers.Customers'
```

**Solution:**

Aspire runs migrations automatically via the `db-migrator` service. Check:
1. Aspire Dashboard > Resources > `db-migrator` status
2. View logs for migration errors

To run migrations manually:
```bash
cd src/backend/Services/Fleet/OrangeCarRental.Fleet.Infrastructure
dotnet ef database update --startup-project ../OrangeCarRental.Fleet.Api
```

---

## Frontend Issues

### Frontend Not Loading (Blank Page)

**Solutions:**

1. Check Aspire Dashboard for frontend service status
2. Clear browser cache: `Ctrl + Shift + R`
3. Check browser console (F12) for errors

### CORS Errors

**Symptom:**
```
Access to fetch blocked by CORS policy
```

**Solution:** Verify frontend origin is in API Gateway CORS config (`src/backend/ApiGateway/Program.cs`)

### npm install Failures

```bash
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

---

## API Errors

### 401 Unauthorized

Check Keycloak is running in Aspire Dashboard and JWT configuration is correct.

### 500 Internal Server Error

1. Check Aspire Dashboard > Logs
2. Filter by the failing service
3. Look for exceptions

### API Returns Empty Results

1. Verify database has data (check via Aspire Dashboard traces)
2. Check query filters in code
3. View SQL queries in Aspire structured logs

---

## Docker Issues

### Docker Desktop Not Running

```
Cannot connect to the Docker daemon
```

1. Start Docker Desktop
2. Wait for whale icon in system tray
3. Verify: `docker version`
4. Restart Aspire

### Container Won't Start

```bash
# Restart Docker Desktop
# Remove conflicting containers
docker container prune -f

# Restart Aspire
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run
```

### Out of Disk Space

```bash
docker system prune -a --volumes
docker builder prune -a
```

---

## Build Failures

### NuGet Restore Fails

```bash
dotnet nuget locals all --clear
dotnet restore
```

### TypeScript Compilation Errors

```bash
cd src/frontend
rm -rf node_modules
yarn install
```

---

## Emergency Recovery

### Complete System Reset

```bash
# 1. Stop everything
taskkill /F /IM dotnet.exe
taskkill /F /IM node.exe
docker stop $(docker ps -q)

# 2. Clean Docker
docker system prune -f

# 3. Clean build artifacts
cd src/backend
dotnet clean

cd ../frontend
rm -rf node_modules

# 4. Reinstall
cd src/backend
dotnet restore

cd ../frontend
yarn install

# 5. Start fresh
cd src/backend/AppHost/OrangeCarRental.AppHost
dotnet run
```

---

## Common Error Messages

| Error | Cause | Fix |
|-------|-------|-----|
| `EADDRINUSE` | Port in use | Kill process using the port |
| `SqlException` | Database issue | Check SQL Server in Aspire Dashboard |
| `401 Unauthorized` | Auth issue | Check Keycloak status |
| `CORS policy` | Origin blocked | Update CORS config |
| `no endpoints configured` | Service starting | Wait or restart service |
| `Invalid object name` | Missing migrations | Check db-migrator logs |

---

## Getting Help

1. **Aspire Dashboard** - Central monitoring for all services
2. **Structured Logs** - Filter by service, level, or search
3. **Distributed Traces** - End-to-end request tracking

**Documentation:**
- [ASPIRE-QUICKSTART.md](./ASPIRE-QUICKSTART.md) - Main development guide
