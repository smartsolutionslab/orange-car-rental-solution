# Local Testing Guide - Orange Car Rental

**Last Updated:** 2025-11-17
**Purpose:** Step-by-step guide for running complete end-to-end tests locally

---

## Prerequisites Checklist

Before starting, ensure you have:

- [x] **.NET 9 SDK** (version 9.0.307 ✅ installed)
- [ ] **SQL Server** (localhost) running with Windows Authentication
- [x] **Node.js 20+** installed
- [ ] **Git Bash** or terminal with bash support

---

## Quick Start

### Option 1: Automated Setup (Recommended)

```bash
# Run this single command to start all services
cd C:\Users\heiko\claude-orange-car-rental\src\backend
chmod +x start-all-services.sh
./start-all-services.sh
```

### Option 2: Manual Step-by-Step

Follow the detailed steps below if you need more control or troubleshooting.

---

## Step-by-Step Manual Setup

### Step 1: Verify SQL Server

**Check if SQL Server is running:**

```powershell
# Open PowerShell and run:
Get-Service MSSQLSERVER
```

**Expected output:**
```
Status   Name           DisplayName
------   ----           -----------
Running  MSSQLSERVER    SQL Server (MSSQLSERVER)
```

**If not running:**
```powershell
Start-Service MSSQLSERVER
```

**Alternative: SQL Server LocalDB**

If you don't have SQL Server, you can use LocalDB:

```bash
# Start LocalDB instance
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

Then update connection strings in all `appsettings.json` files to:
```json
"Server=(localdb)\\MSSQLLocalDB;Database=OrangeCarRental_Fleet;Integrated Security=true;TrustServerCertificate=true;"
```

---

### Step 2: Run Database Migrations

Navigate to backend directory and run migrations for each service:

```bash
cd C:\Users\heiko\claude-orange-car-rental\src\backend
```

**Fleet Service:**
```bash
cd Services/Fleet/OrangeCarRental.Fleet.Infrastructure
dotnet ef database update --startup-project ../OrangeCarRental.Fleet.Api

cd ../../..
```

**Reservations Service:**
```bash
cd Services/Reservations/OrangeCarRental.Reservations.Infrastructure
dotnet ef database update --startup-project ../OrangeCarRental.Reservations.Api

cd ../../..
```

**Customers Service:**
```bash
cd Services/Customers/OrangeCarRental.Customers.Infrastructure
dotnet ef database update --startup-project ../OrangeCarRental.Customers.Api

cd ../../..
```

**Pricing Service:**
```bash
cd Services/Pricing/OrangeCarRental.Pricing.Infrastructure
dotnet ef database update --startup-project ../OrangeCarRental.Pricing.Api

cd ../../..
```

**Location Service:**
```bash
cd Services/Location/OrangeCarRental.Location.Infrastructure
dotnet ef database update --startup-project ../OrangeCarRental.Location.Api

cd ../../..
```

**Notifications Service (if needed):**
```bash
cd Services/Notifications/OrangeCarRental.Notifications.Infrastructure
dotnet ef database update --startup-project ../OrangeCarRental.Notifications.Api

cd ../../..
```

**Payments Service (if needed):**
```bash
cd Services/Payments/OrangeCarRental.Payments.Infrastructure
dotnet ef database update --startup-project ../OrangeCarRental.Payments.Api

cd ../../..
```

**Expected output for each:**
```
Build started...
Build succeeded.
Done.
```

**Verify databases created:**
```powershell
sqlcmd -S localhost -E -Q "SELECT name FROM sys.databases WHERE name LIKE 'OrangeCarRental%'"
```

Expected databases:
- OrangeCarRental_Fleet
- OrangeCarRental_Reservations
- OrangeCarRental_Customers
- OrangeCarRental_Pricing
- OrangeCarRental_Location
- OrangeCarRental_Notifications (optional)
- OrangeCarRental_Payments (optional)

---

### Step 3: Start Backend Services

**Build all projects first:**
```bash
cd C:\Users\heiko\claude-orange-car-rental\src\backend
dotnet build --nologo --verbosity quiet
```

**Start services using the automated script:**
```bash
chmod +x start-all-services.sh
./start-all-services.sh
```

**Services will start on:**
- API Gateway: http://localhost:5002
- Fleet API: http://localhost:5000
- Reservations API: http://localhost:5001
- Customers API: http://localhost:5003
- Payments API: http://localhost:5004
- Notifications API: http://localhost:5005
- Locations API: http://localhost:5006

**View service logs:**
```bash
tail -f logs/*.log
```

**Check health status:**
```bash
# After services start (wait ~45 seconds), check health
curl http://localhost:5002/health  # API Gateway
curl http://localhost:5000/health  # Fleet
curl http://localhost:5001/health  # Reservations
curl http://localhost:5003/health  # Customers
```

Expected response: `{"status":"Healthy"}`

---

### Step 4: Start Frontend

**Open a new terminal:**

```bash
cd C:\Users\heiko\claude-orange-car-rental\src\frontend\apps\public-portal

# Install dependencies (first time only)
npm install

# Start development server
npm start
```

**Expected output:**
```
✔ Browser application bundle generation complete.
** Angular Live Development Server is listening on localhost:4200 **
```

**Open browser:**
- Public Portal: http://localhost:4200

---

## End-to-End Test Scenarios

### Test 1: Health Check ✅

**Verify all services are healthy:**

```bash
# Check all health endpoints
curl http://localhost:5000/health
curl http://localhost:5001/health
curl http://localhost:5003/health
curl http://localhost:5004/health
curl http://localhost:5005/health
curl http://localhost:5006/health
```

**Expected:** All return `{"status":"Healthy"}`

---

### Test 2: API Documentation 📚

**Open Scalar API docs in browser:**

- Fleet API: http://localhost:5000/scalar/v1
- Reservations API: http://localhost:5001/scalar/v1
- Customers API: http://localhost:5003/scalar/v1

**Verify:** Interactive API documentation loads

---

### Test 3: Vehicle Search Flow 🚗

**Using the Frontend UI:**

1. Open: http://localhost:4200
2. Select pickup location (e.g., "Berlin HBF")
3. Select pickup date (tomorrow)
4. Select return date (3 days from now)
5. Click "Fahrzeuge suchen" (Search vehicles)

**Expected Results:**
- Loading indicator appears
- Vehicle cards display with:
  - Vehicle name and category
  - Daily rate with German VAT (19%)
  - Currency formatted as "59,50 €"
  - Available status
  - "Jetzt buchen" button

**Using API Directly:**

```bash
curl -X GET "http://localhost:5000/api/vehicles?locationCode=BER-HBF" \
  -H "accept: application/json"
```

**Expected:** JSON array of vehicles

---

### Test 4: Guest Booking Flow 📝

**Prerequisites:** Test 3 completed (vehicles displayed)

**Steps:**
1. Click "Jetzt buchen" on any vehicle
2. Fill booking form:
   - Salutation: "Herr" or "Frau"
   - First name: "Max"
   - Last name: "Mustermann"
   - Email: "max@example.com"
   - Phone: "+49 30 1234567"
   - Date of birth: 01.01.1990 (min 18 years old)
   - Driver's license number: "AB123456789"
   - License issue date: At least 30 days ago
   - License expiry date: At least 30 days in future
   - Street + Number: "Hauptstraße 1"
   - Postal code: "10115" (5 digits)
   - City: "Berlin"
   - Country: "Deutschland"
3. Click "Reservierung abschließen"

**Expected Results:**
- Form validation works:
  - Email format validated
  - Phone number accepts German formats
  - Age minimum 18 years enforced
  - License validity minimum 30 days enforced
- Successful booking redirects to confirmation page
- Confirmation shows:
  - Reservation number (GUID)
  - Vehicle details
  - Booking period
  - Total price with VAT breakdown
  - Customer details

**API Verification:**

```bash
curl -X POST "http://localhost:5001/api/reservations/guest" \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": "PUT_VEHICLE_ID_HERE",
    "pickupDate": "2025-11-18",
    "returnDate": "2025-11-21",
    "pickupLocationCode": "BER-HBF",
    "returnLocationCode": "BER-HBF",
    "customerSalutation": "Herr",
    "customerFirstName": "Max",
    "customerLastName": "Mustermann",
    "customerEmail": "max@example.com",
    "customerPhone": "+49301234567",
    "customerDateOfBirth": "1990-01-01",
    "customerStreet": "Hauptstraße 1",
    "customerPostalCode": "10115",
    "customerCity": "Berlin",
    "customerCountry": "Deutschland",
    "driversLicenseNumber": "AB123456789",
    "driversLicenseIssueDate": "2020-01-01",
    "driversLicenseExpiryDate": "2030-01-01"
  }'
```

**Expected:** 201 Created with reservation details

---

### Test 5: Price Calculation 💰

**Test German VAT calculation:**

```bash
curl -X POST "http://localhost:5004/api/pricing/calculate" \
  -H "Content-Type: application/json" \
  -d '{
    "categoryCode": "MITTEL",
    "pickupDate": "2025-11-18",
    "returnDate": "2025-11-21",
    "locationCode": "BER-HBF"
  }'
```

**Expected Response:**
```json
{
  "netAmount": 150.00,
  "vatAmount": 28.50,
  "grossAmount": 178.50,
  "currency": "EUR",
  "rentalDays": 3
}
```

**Verify:**
- VAT is exactly 19% of net amount
- Gross = Net + VAT
- German decimal formatting: "178,50 €"

---

### Test 6: Customer Registration 👤

**Via Frontend:**

1. Navigate to registration page
2. Fill form with valid data
3. Submit registration

**Via API:**

```bash
curl -X POST "http://localhost:5003/api/customers/register" \
  -H "Content-Type: application/json" \
  -d '{
    "salutation": "Frau",
    "firstName": "Anna",
    "lastName": "Schmidt",
    "email": "anna.schmidt@example.com",
    "phone": "+49 89 1234567",
    "dateOfBirth": "1995-05-15",
    "street": "Marienplatz 8",
    "postalCode": "80331",
    "city": "München",
    "country": "Deutschland",
    "driversLicenseNumber": "MU987654321",
    "driversLicenseIssueDate": "2018-06-01",
    "driversLicenseExpiryDate": "2028-06-01"
  }'
```

**Expected:**
- 201 Created
- Response includes customer ID (GUID)
- Email normalized to lowercase
- Phone normalized to +49 format

---

### Test 7: Error Handling ⚠️

**Test validation errors:**

```bash
# Invalid email
curl -X POST "http://localhost:5001/api/reservations/guest" \
  -H "Content-Type: application/json" \
  -d '{
    "customerEmail": "invalid-email"
  }'
```

**Expected:** 400 Bad Request with validation message

**Test not found:**

```bash
curl -X GET "http://localhost:5001/api/reservations/00000000-0000-0000-0000-000000000000"
```

**Expected:** 404 Not Found

**Frontend error display:**
1. Enter invalid email in booking form
2. Verify error message displays in German
3. Check error is highlighted visually

---

## Performance Testing (Basic)

### Test Response Times

**Measure API response times:**

```bash
# Fleet API - Vehicle search
time curl -s http://localhost:5000/api/vehicles > /dev/null

# Reservations API - Health check
time curl -s http://localhost:5001/health > /dev/null
```

**Expected:**
- Health checks: < 100ms
- Vehicle search: < 500ms

### Test Frontend Load Time

1. Open browser dev tools (F12)
2. Go to Network tab
3. Navigate to http://localhost:4200
4. Check "Load" time in footer

**Expected:** < 3 seconds for initial load

---

## German Market Compliance Verification

### Test German Formatting

**Dates:**
- Format: DD.MM.YYYY
- Example: 17.11.2025

**Currency:**
- Format: 1.234,56 €
- Decimal separator: comma (,)
- Thousands separator: period (.)

**VAT:**
- Standard rate: 19%
- Net + VAT = Gross
- All three amounts visible

**Phone Numbers:**
- Accepts: +49 30 1234567
- Accepts: 030 1234567
- Accepts: +49-30-1234567
- Normalizes to: +49301234567

**Postal Codes:**
- 5 digits required
- Example: 10115

### Test Age Validation

**Minimum age: 18 years**

```bash
# Test with underage customer (should fail)
curl -X POST "http://localhost:5003/api/customers/register" \
  -H "Content-Type: application/json" \
  -d '{
    "dateOfBirth": "'$(date -d '15 years ago' +%Y-%m-%d)'"
  }'
```

**Expected:** 400 Bad Request with age validation error

### Test License Validity

**Minimum validity: 30 days**

```bash
# Test with expiring license (should fail)
curl -X POST "http://localhost:5003/api/customers/register" \
  -H "Content-Type: application/json" \
  -d '{
    "driversLicenseExpiryDate": "'$(date -d '+20 days' +%Y-%m-%d)'"
  }'
```

**Expected:** 400 Bad Request with license validity error

---

## Troubleshooting

### Services won't start

**Check port conflicts:**
```bash
netstat -ano | findstr ":5000"
netstat -ano | findstr ":5001"
netstat -ano | findstr ":5003"
```

**Kill processes using ports:**
```powershell
taskkill /PID <PID> /F
```

**Check logs:**
```bash
cat logs/Fleet-API.log
cat logs/Reservations-API.log
```

### Database connection fails

**Verify SQL Server running:**
```powershell
Get-Service MSSQLSERVER
```

**Test connection:**
```bash
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

**Check connection string:**
- Server=localhost
- Integrated Security=true
- TrustServerCertificate=true

### Frontend won't load

**Check Node.js version:**
```bash
node --version  # Should be 20+
```

**Clear npm cache:**
```bash
cd src/frontend/apps/public-portal
rm -rf node_modules
npm cache clean --force
npm install
```

**Check API connection:**
- Open browser dev tools → Network tab
- Look for failed API calls
- Verify backend services are running

---

## Stop All Services

**Backend:**
```bash
cd C:\Users\heiko\claude-orange-car-rental\src\backend
./stop-all-services.sh
```

**Frontend:**
Press `Ctrl+C` in the terminal running `npm start`

**Verify stopped:**
```bash
netstat -ano | findstr ":5000"
netstat -ano | findstr ":4200"
```

---

## Test Checklist

Use this checklist to track your testing progress:

### Infrastructure
- [ ] SQL Server running
- [ ] Databases created (7 databases)
- [ ] Backend services healthy (7 services)
- [ ] Frontend running

### Functional Tests
- [ ] Vehicle search works
- [ ] Vehicle filtering works
- [ ] Guest booking creates reservation
- [ ] Price calculation correct (VAT 19%)
- [ ] Customer registration works
- [ ] Confirmation page displays

### German Market
- [ ] Dates in DD.MM.YYYY format
- [ ] Currency as "X.XXX,XX €"
- [ ] VAT 19% calculated correctly
- [ ] Age minimum 18 years enforced
- [ ] License minimum 30 days enforced
- [ ] Phone numbers accept German formats

### Error Handling
- [ ] Invalid email rejected
- [ ] Invalid phone rejected
- [ ] Underage customer rejected
- [ ] Expired license rejected
- [ ] 404 for non-existent resources
- [ ] Error messages in German

### Performance
- [ ] Health checks < 100ms
- [ ] Vehicle search < 500ms
- [ ] Frontend loads < 3s

---

## Next Steps After Local Testing

Once all local tests pass:

1. **Document Results:**
   - Create test report
   - Note any issues found
   - Record performance metrics

2. **Azure Deployment:**
   - Provision Azure resources
   - Deploy to staging environment
   - Run tests again in cloud

3. **Production Launch:**
   - Final smoke tests
   - Enable monitoring
   - Go live!

---

**Last Updated:** 2025-11-17
**Tested By:** [Your Name]
**Status:** Ready for local testing
