# Keycloak Setup Guide for Orange Car Rental

This guide explains how to set up Keycloak for authentication and authorization in the Orange Car Rental system.

## Prerequisites

- .NET 9.0 SDK installed
- .NET Aspire workload installed
- Port 8080 available for Keycloak

## 1. Start Keycloak via Aspire

Keycloak is automatically started when you run the Aspire AppHost:

```bash
cd src/backend
dotnet run --project AppHost/OrangeCarRental.AppHost
```

Aspire will automatically:
- Pull the Keycloak container image (quay.io/keycloak/keycloak:26.0.7)
- Start Keycloak on port 8080
- Configure admin credentials
- Ensure all services wait for Keycloak to be ready

Wait for Keycloak to be ready (check the Aspire Dashboard at http://localhost:15xxx).

Access the Keycloak admin console:
- **URL**: http://localhost:8080
- **Username**: `admin`
- **Password**: `admin`

## 2. Create the Orange Car Rental Realm

1. Log in to Keycloak admin console
2. Click on the dropdown in the top-left corner (currently showing "master")
3. Click "Create Realm"
4. Set **Realm name**: `orange-car-rental`
5. Click "Create"

## 3. Configure Realm Settings

1. Go to **Realm Settings** → **Login**
   - Enable "User registration": `ON`
   - Enable "Forgot password": `ON`
   - Enable "Remember me": `ON`

2. Go to **Realm Settings** → **Tokens**
   - Set Access Token Lifespan: `15 minutes`
   - Set Refresh Token Lifespan: `30 minutes`
   - Set SSO Session Idle: `30 minutes`
   - Set SSO Session Max: `10 hours`

## 4. Create Realm Roles

Go to **Realm Roles** and create the following roles:

1. **customer**
   - Description: "Public users who can search vehicles and make reservations"

2. **call_center**
   - Description: "Staff who can manage customer reservations"

3. **admin**
   - Description: "Full system access for administration"

4. **fleet_manager**
   - Description: "Manage vehicle fleet and locations"

## 5. Create Clients

### 5.1 Public Portal Client (Angular SPA)

1. Go to **Clients** → **Create client**
2. Configure:
   - **Client ID**: `public-portal`
   - **Client type**: `OpenID Connect`
   - **Client authentication**: `OFF` (public client)
   - Click "Next"
3. Configure Capability:
   - **Standard flow**: `ON`
   - **Direct access grants**: `ON`
   - Click "Next"
4. Configure Login settings:
   - **Valid redirect URIs**: `http://localhost:4300/*`
   - **Valid post logout redirect URIs**: `http://localhost:4300/*`
   - **Web origins**: `http://localhost:4300`
   - Click "Save"

### 5.2 Call Center Portal Client (Angular SPA)

1. Create client with ID: `call-center-portal`
2. Same configuration as public-portal but use:
   - **Valid redirect URIs**: `http://localhost:4302/*`
   - **Valid post logout redirect URIs**: `http://localhost:4302/*`
   - **Web origins**: `http://localhost:4302`

### 5.3 Backend API Client (Confidential)

1. Go to **Clients** → **Create client**
2. Configure:
   - **Client ID**: `orange-car-rental-api`
   - **Client type**: `OpenID Connect`
   - **Client authentication**: `ON` (confidential client)
   - Click "Next"
3. Configure Capability:
   - **Service accounts roles**: `ON`
   - Click "Save"
4. Go to **Credentials** tab
   - Note the **Client secret** (you'll need this for backend configuration)

## 6. Create Test Users

Go to **Users** → **Create new user**

### 6.1 Test Customer
- **Username**: `test.customer`
- **Email**: `customer@test.com`
- **First name**: `Test`
- **Last name**: `Customer`
- **Email verified**: `ON`
- Click "Create"
- Go to **Credentials** tab → Set password: `customer123`
- **Temporary**: `OFF`
- Go to **Role mapping** tab → **Assign role** → Select `customer`

### 6.2 Test Call Center Agent
- **Username**: `test.agent`
- **Email**: `agent@test.com`
- **First name**: `Test`
- **Last name**: `Agent`
- **Email verified**: `ON`
- Click "Create"
- Set password: `agent123` (temporary: OFF)
- Assign roles: `call_center`, `customer`

### 6.3 Test Admin
- **Username**: `test.admin`
- **Email**: `admin@test.com`
- **First name**: `Test`
- **Last name**: `Admin`
- **Email verified**: `ON`
- Click "Create"
- Set password: `admin123` (temporary: OFF)
- Assign roles: `admin`, `call_center`, `customer`

### 6.4 Test Fleet Manager
- **Username**: `test.fleet`
- **Email**: `fleet@test.com`
- **First name**: `Test`
- **Last name**: `FleetManager`
- **Email verified**: `ON`
- Click "Create"
- Set password: `fleet123` (temporary: OFF)
- Assign roles: `fleet_manager`, `call_center`, `customer`

## 7. Get Configuration Details

You'll need these values for backend configuration:

1. Go to **Realm Settings** → **OpenID Endpoint Configuration**
   - Copy the JSON or note these URLs:
   ```
   Authority: http://localhost:8080/realms/orange-car-rental
   Token endpoint: http://localhost:8080/realms/orange-car-rental/protocol/openid-connect/token
   ```

2. Client secret (from Backend API Client):
   - Go to **Clients** → **orange-car-rental-api** → **Credentials**
   - Copy the **Client secret**

## 8. Backend Configuration

Add to your `appsettings.json` (or `appsettings.Development.json`):

```json
{
  "Authentication": {
    "Keycloak": {
      "Authority": "http://localhost:8080/realms/orange-car-rental",
      "Audience": "orange-car-rental-api",
      "RequireHttpsMetadata": false,
      "ValidateIssuer": true,
      "ValidateAudience": true,
      "ValidateLifetime": true
    }
  }
}
```

## 9. Frontend Configuration

Add to your Angular environment files:

```typescript
export const environment = {
  production: false,
  keycloak: {
    url: 'http://localhost:8080',
    realm: 'orange-car-rental',
    clientId: 'public-portal', // or 'call-center-portal'
  },
  apiUrl: 'http://localhost:5000'
};
```

## 10. Role Permissions Matrix

| Feature | Customer | Call Center | Fleet Manager | Admin |
|---------|----------|-------------|---------------|-------|
| Search vehicles | ✅ | ✅ | ✅ | ✅ |
| View own reservations | ✅ | ✅ | ✅ | ✅ |
| Create reservation (self) | ✅ | ✅ | ✅ | ✅ |
| Create reservation (others) | ❌ | ✅ | ❌ | ✅ |
| View all reservations | ❌ | ✅ | ❌ | ✅ |
| Cancel any reservation | ❌ | ✅ | ❌ | ✅ |
| View all customers | ❌ | ✅ | ❌ | ✅ |
| Manage vehicles | ❌ | ❌ | ✅ | ✅ |
| Manage locations | ❌ | ❌ | ✅ | ✅ |
| Manage pricing | ❌ | ❌ | ❌ | ✅ |
| System administration | ❌ | ❌ | ❌ | ✅ |

## 11. Testing Authentication

### Get Access Token (for testing)

```bash
curl -X POST 'http://localhost:8080/realms/orange-car-rental/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'client_id=public-portal' \
  -d 'username=test.customer' \
  -d 'password=customer123' \
  -d 'grant_type=password'
```

### Use Access Token

```bash
curl -X GET 'http://localhost:5000/api/fleet/vehicles' \
  -H 'Authorization: Bearer YOUR_ACCESS_TOKEN_HERE'
```

## 12. Production Considerations

Before deploying to production:

1. ⚠️ Change admin password from `admin`
2. ⚠️ Use HTTPS for all communication
3. ⚠️ Set `KC_HOSTNAME` to your production domain
4. ⚠️ Enable `KC_HOSTNAME_STRICT=true`
5. ⚠️ Use proper SQL Server credentials
6. ⚠️ Configure email server for password resets
7. ⚠️ Set up backup for Keycloak database
8. ⚠️ Configure session timeouts appropriately
9. ⚠️ Enable MFA/2FA for admin users
10. ⚠️ Review and harden security settings

## Troubleshooting

### Keycloak won't start
- Check if port 8080 is already in use
- Check the Aspire Dashboard for Keycloak container status
- View Keycloak logs in the Aspire Dashboard console tab

### Cannot access admin console
- Wait for Keycloak to fully start (can take 30-60 seconds)
- Check the Aspire Dashboard to verify Keycloak is running and healthy
- Ensure port 8080 is not blocked by firewall

### Token validation fails
- Ensure `Authority` URL is correct and accessible from backend
- Check that realm name matches exactly (`orange-car-rental`)
- Verify client is configured correctly
- Check that the backend appsettings.Development.json has the correct Keycloak configuration

### Services can't connect to Keycloak
- Verify Keycloak is running in the Aspire Dashboard
- Check that all services have `.WithReference(keycloak)` in AppHost/Program.cs
- Ensure the appsettings.Development.json files point to `http://localhost:8080`

## Next Steps

After completing this setup, proceed to:
1. Configure backend APIs with JWT authentication
2. Implement Angular authentication service
3. Add authorization policies to endpoints
4. Test end-to-end authentication flow
