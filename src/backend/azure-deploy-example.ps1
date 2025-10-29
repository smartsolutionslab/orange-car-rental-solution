# Example Azure Deployment Script for Orange Car Rental
# This demonstrates how to deploy the application to Azure Container Apps with proper migration handling

param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroup,

    [Parameter(Mandatory=$true)]
    [string]$Location,

    [Parameter(Mandatory=$true)]
    [string]$ContainerRegistry,

    [Parameter(Mandatory=$true)]
    [string]$SqlServer,

    [Parameter(Mandatory=$true)]
    [string]$SqlAdminPassword,

    [string]$Environment = "Production"
)

Write-Host "🚀 Deploying Orange Car Rental to Azure" -ForegroundColor Green

# Variables
$imageTag = "latest"
$fleetApiImage = "$ContainerRegistry.azurecr.io/fleet-api:$imageTag"
$reservationsApiImage = "$ContainerRegistry.azurecr.io/reservations-api:$imageTag"

# Connection strings
$fleetConnectionString = "Server=tcp:$SqlServer.database.windows.net,1433;Database=OrangeCarRental_Fleet;User ID=sqladmin;Password=$SqlAdminPassword;Encrypt=True;TrustServerCertificate=False;"
$reservationsConnectionString = "Server=tcp:$SqlServer.database.windows.net,1433;Database=OrangeCarRental_Reservations;User ID=sqladmin;Password=$SqlAdminPassword;Encrypt=True;TrustServerCertificate=False;"

# Step 1: Build and push container images
Write-Host "`n📦 Building and pushing container images..." -ForegroundColor Cyan

docker build -t $fleetApiImage -f Services/Fleet/OrangeCarRental.Fleet.Api/Dockerfile .
docker push $fleetApiImage

docker build -t $reservationsApiImage -f Services/Reservations/OrangeCarRental.Reservations.Api/Dockerfile .
docker push $reservationsApiImage

# Step 2: Run database migrations
Write-Host "`n🔄 Running database migrations..." -ForegroundColor Cyan

Write-Host "  📊 Fleet database migration..." -ForegroundColor Yellow
az containerapp job create `
    --name "fleet-migration-job" `
    --resource-group $ResourceGroup `
    --image $fleetApiImage `
    --cpu 0.5 --memory 1.0Gi `
    --trigger-type Manual `
    --replica-timeout 300 `
    --args "--migrate-only" `
    --env-vars `
        "ConnectionStrings__fleet=$fleetConnectionString" `
        "ASPNETCORE_ENVIRONMENT=$Environment" `
    --registry-server "$ContainerRegistry.azurecr.io"

az containerapp job start --name "fleet-migration-job" --resource-group $ResourceGroup
Write-Host "  ✅ Fleet migrations completed" -ForegroundColor Green

Write-Host "  📊 Reservations database migration..." -ForegroundColor Yellow
az containerapp job create `
    --name "reservations-migration-job" `
    --resource-group $ResourceGroup `
    --image $reservationsApiImage `
    --cpu 0.5 --memory 1.0Gi `
    --trigger-type Manual `
    --replica-timeout 300 `
    --args "--migrate-only" `
    --env-vars `
        "ConnectionStrings__reservations=$reservationsConnectionString" `
        "ASPNETCORE_ENVIRONMENT=$Environment" `
    --registry-server "$ContainerRegistry.azurecr.io"

az containerapp job start --name "reservations-migration-job" --resource-group $ResourceGroup
Write-Host "  ✅ Reservations migrations completed" -ForegroundColor Green

# Step 3: Deploy or update Container Apps
Write-Host "`n🌐 Deploying services..." -ForegroundColor Cyan

# Fleet API
Write-Host "  🚗 Deploying Fleet API..." -ForegroundColor Yellow
az containerapp create `
    --name "fleet-api" `
    --resource-group $ResourceGroup `
    --image $fleetApiImage `
    --cpu 0.5 --memory 1.0Gi `
    --min-replicas 1 --max-replicas 3 `
    --ingress external --target-port 8080 `
    --env-vars `
        "ConnectionStrings__fleet=$fleetConnectionString" `
        "ASPNETCORE_ENVIRONMENT=$Environment" `
        "Database__AutoMigrate=false" `
    --registry-server "$ContainerRegistry.azurecr.io"

# Reservations API
Write-Host "  📅 Deploying Reservations API..." -ForegroundColor Yellow
az containerapp create `
    --name "reservations-api" `
    --resource-group $ResourceGroup `
    --image $reservationsApiImage `
    --cpu 0.5 --memory 1.0Gi `
    --min-replicas 1 --max-replicas 3 `
    --ingress external --target-port 8080 `
    --env-vars `
        "ConnectionStrings__reservations=$reservationsConnectionString" `
        "ASPNETCORE_ENVIRONMENT=$Environment" `
        "Database__AutoMigrate=false" `
    --registry-server "$ContainerRegistry.azurecr.io"

Write-Host "`n✅ Deployment completed successfully!" -ForegroundColor Green
Write-Host "`nEndpoints:" -ForegroundColor Cyan
az containerapp show --name "fleet-api" --resource-group $ResourceGroup --query "properties.configuration.ingress.fqdn" -o tsv
az containerapp show --name "reservations-api" --resource-group $ResourceGroup --query "properties.configuration.ingress.fqdn" -o tsv

# Usage Example:
# .\azure-deploy-example.ps1 `
#     -ResourceGroup "rg-orange-car-rental" `
#     -Location "westeurope" `
#     -ContainerRegistry "crorgrental" `
#     -SqlServer "sql-orgrental" `
#     -SqlAdminPassword "YourSecurePassword123!"
