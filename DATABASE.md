# Orange Car Rental - Database Setup Guide

Complete guide for database setup, migrations, and seed data for the Orange Car Rental microservices architecture.

## Table of Contents

- [Overview](#overview)
- [Quick Start](#quick-start)
- [Database Options](#database-options)
- [Database Structure](#database-structure)
- [Setup Instructions](#setup-instructions)
- [Migrations](#migrations)
- [Seed Data](#seed-data)
- [Troubleshooting](#troubleshooting)

## Overview

The Orange Car Rental system uses a **database-per-service** pattern with 7 separate databases:

| Service | Database Name | Purpose |
|---------|--------------|---------|
| Customers | `orange_rental_customers` | Customer information and profiles |
| Fleet | `orange_rental_fleet` | Vehicle inventory and management |
| Location | `orange_rental_location` | Rental locations and branches |
| Reservations | `orange_rental_reservations` | Booking and reservation data |
| Pricing | `orange_rental_pricing` | Pricing rules and calculations |
| Payments | `orange_rental_payments` | Payment transactions |
| Notifications | `orange_rental_notifications` | Notification templates and logs |

## Quick Start

### Option 1: PostgreSQL (Recommended)

```bash
# 1. Start PostgreSQL and PgAdmin
docker-compose -f docker-compose.database.yml up -d

# 2. Run migrations (creates tables)
cd src/backend
dotnet ef database update --project Services/Customers/OrangeCarRental.Customers.Api
dotnet ef database update --project Services/Fleet/OrangeCarRental.Fleet.Api
dotnet ef database update --project Services/Location/OrangeCarRental.Location.Api
dotnet ef database update --project Services/Reservations/OrangeCarRental.Reservations.Api
dotnet ef database update --project Services/Pricing/OrangeCarRental.Pricing.Api
dotnet ef database update --project Services/Payments/OrangeCarRental.Payments.Api
dotnet ef database update --project Services/Notifications/OrangeCarRental.Notifications.Api

# Or use the migration script
powershell -ExecutionPolicy Bypass -File .\scripts\db\run-migrations.ps1

# 3. Apply seed data (optional)
powershell -ExecutionPolicy Bypass -File .\scripts\db\seed-data.ps1
```

Access:
- **PostgreSQL**: `localhost:5432`
- **PgAdmin**: `http://localhost:5050` (admin@orangecarrental.com / admin)

### Option 2: SQL Server

```bash
# 1. Start SQL Server
docker-compose -f docker-compose.sqlserver.yml up -d

# 2. Update connection strings in appsettings.Development.json for each service
# 3. Run migrations (same as above)
```

Access:
- **SQL Server**: `localhost,1433` (sa / YourStrong@Passw0rd)

## Database Options

### PostgreSQL Setup

**File**: `docker-compose.database.yml`

**Features**:
- PostgreSQL 15 Alpine
- PgAdmin 4 for database management
- Auto-creates 7 microservice databases
- Healthchecks for container orchestration
- Persistent volumes for data

**Connection String**:
```
Host=localhost;Port=5432;Database=orange_rental_[service];Username=orange_user;Password=orange_dev_password
```

### SQL Server Setup

**File**: `docker-compose.sqlserver.yml`

**Features**:
- SQL Server 2022 Developer Edition
- Auto-creates 7 microservice databases
- Persistent volumes for data

**Connection String**:
```
Server=localhost,1433;Database=OrangeCarRental_[Service];User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

## Database Structure

Each microservice has its own database with tables managed by EF Core migrations:

### Customers Database
- `Customers` - Customer profiles
- Driver's license information
- Contact details

### Fleet Database
- `Vehicles` - Vehicle inventory
- Vehicle specifications
- Availability status

### Location Database
- `Locations` - Rental locations
- Operating hours
- Contact information

### Reservations Database
- `Reservations` - Booking records
- Pickup/return information
- Reservation status

### Pricing Database
- `PricingRules` - Dynamic pricing
- Seasonal rates
- Discount configurations

### Payments Database
- `Payments` - Payment records
- Transaction history
- Payment methods

### Notifications Database
- `NotificationTemplates` - Message templates
- `NotificationLogs` - Sent notifications
- Delivery status

## Setup Instructions

### Prerequisites

1. **Docker Desktop** - For running database containers
2. **.NET 9 SDK** - For running migrations
3. **EF Core Tools** - Install globally:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

### Step 1: Start Database

Choose either PostgreSQL or SQL Server:

#### PostgreSQL:
```bash
docker-compose -f docker-compose.database.yml up -d
```

#### SQL Server:
```bash
docker-compose -f docker-compose.sqlserver.yml up -d
```

### Step 2: Verify Database is Running

#### PostgreSQL:
```bash
docker exec orange-rental-db-complete psql -U orange_user -d orange_rental -c "\l"
```

You should see 8 databases listed (orange_rental + 7 microservice databases).

#### SQL Server:
```bash
docker exec orange-rental-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "SELECT name FROM sys.databases"
```

### Step 3: Configure Connection Strings

Update `appsettings.Development.json` in each service's API project:

#### PostgreSQL Example:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=orange_rental_customers;Username=orange_user;Password=orange_dev_password"
  }
}
```

#### SQL Server Example:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrangeCarRental_Customers;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

## Migrations

### Run All Migrations (Automated)

#### Windows (PowerShell):
```powershell
.\scripts\db\run-migrations.ps1
```

#### Linux/Mac (Bash):
```bash
./scripts/db/run-migrations.sh
```

### Run Migrations Manually

For each service:

```bash
cd src/backend/Services/[ServiceName]/OrangeCarRental.[ServiceName].Api
dotnet ef database update
```

Example:
```bash
cd src/backend/Services/Customers/OrangeCarRental.Customers.Api
dotnet ef database update
```

### Create New Migration

```bash
cd src/backend/Services/[ServiceName]/OrangeCarRental.[ServiceName].Api
dotnet ef migrations add [MigrationName]
```

Example:
```bash
cd src/backend/Services/Customers/OrangeCarRental.Customers.Api
dotnet ef migrations add AddCustomerLoyaltyProgram
```

### Rollback Migration

```bash
cd src/backend/Services/[ServiceName]/OrangeCarRental.[ServiceName].Api
dotnet ef database update [PreviousMigrationName]
```

## Seed Data

### Apply Test Data (Automated)

#### Windows (PowerShell):
```powershell
.\scripts\db\seed-data.ps1
```

#### Linux/Mac (Bash):
```bash
./scripts/db/seed-data.sh
```

### Apply Test Data (Docker)

```bash
docker exec -i orange-rental-db-complete psql -U orange_user -d orange_rental < scripts/db/seed-test-data.sql
```

### Test Data Includes

- **5 Customers** - Sample customer profiles
- **10 Vehicles** - Various vehicle categories
- **8 Reservations** - Different reservation states
- **5 Locations** - Major German cities
- Sample data for testing booking flows

## Troubleshooting

### Database Container Won't Start

1. Check if port is already in use:
   ```bash
   netstat -ano | findstr :5432  # PostgreSQL
   netstat -ano | findstr :1433  # SQL Server
   ```

2. Remove existing volumes and restart:
   ```bash
   docker-compose -f docker-compose.database.yml down -v
   docker-compose -f docker-compose.database.yml up -d
   ```

### Migration Fails

1. Check database connection:
   ```bash
   # PostgreSQL
   docker exec orange-rental-db-complete psql -U orange_user -d orange_rental -c "SELECT version();"
   ```

2. Verify connection string in appsettings.Development.json

3. Check EF Core tools are installed:
   ```bash
   dotnet ef --version
   ```

### PgAdmin Can't Connect

1. Check container is running:
   ```bash
   docker ps | findstr pgadmin
   ```

2. Default credentials:
   - Email: `admin@orangecarrental.com`
   - Password: `admin`

3. Server connection details:
   - Host: `postgres` (when using docker network) or `localhost` (from host)
   - Port: `5432`
   - Username: `orange_user`
   - Password: `orange_dev_password`

### Seed Data Fails

1. Ensure migrations have been run first (tables must exist)
2. Check database name matches the connection string
3. Verify PostgreSQL is accessible:
   ```bash
   docker logs orange-rental-db-complete
   ```

## Database Management Tools

### PgAdmin (PostgreSQL)

- **URL**: `http://localhost:5050`
- **Email**: `admin@orangecarrental.com`
- **Password**: `admin`

### SQL Server Management Studio (SSMS)

- **Server**: `localhost,1433`
- **Authentication**: SQL Server Authentication
- **Login**: `sa`
- **Password**: `YourStrong@Passw0rd`

### Command Line Tools

#### PostgreSQL (psql):
```bash
# Connect to database
docker exec -it orange-rental-db-complete psql -U orange_user -d orange_rental

# List databases
\l

# Connect to specific database
\c orange_rental_customers

# List tables
\dt

# Exit
\q
```

#### SQL Server (sqlcmd):
```bash
# Connect to database
docker exec -it orange-rental-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd

# List databases
SELECT name FROM sys.databases;
GO

# Use database
USE OrangeCarRental_Customers;
GO

# List tables
SELECT * FROM INFORMATION_SCHEMA.TABLES;
GO

# Exit
EXIT
```

## Production Considerations

### Security

1. **Change default passwords** in production
2. **Use secrets management** (Azure Key Vault, AWS Secrets Manager)
3. **Enable SSL/TLS** for database connections
4. **Restrict network access** using firewalls and security groups
5. **Use connection pooling** appropriately

### Performance

1. **Index optimization** - Review EF Core generated indexes
2. **Connection pooling** - Configure max pool size
3. **Query optimization** - Use EF Core query logging
4. **Monitoring** - Prometheus metrics for database connections

### Backup Strategy

1. **Automated backups** - Daily full, hourly incremental
2. **Backup retention** - 30 days minimum
3. **Test restores** - Monthly restore tests
4. **Disaster recovery** - Cross-region replication

### Scaling

1. **Read replicas** - For read-heavy microservices
2. **Connection pooling** - Optimize connections per service
3. **Database monitoring** - Track query performance
4. **Vertical scaling** - Increase database resources as needed

## Additional Resources

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [SQL Server Documentation](https://docs.microsoft.com/en-us/sql/sql-server/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Docker Compose](https://docs.docker.com/compose/)

## Support

For issues or questions:
1. Check logs: `docker logs orange-rental-db-complete`
2. Review migrations: `dotnet ef migrations list`
3. Validate connection strings
4. Check database server status
