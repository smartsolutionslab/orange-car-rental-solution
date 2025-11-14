# Build stage - Alpine for smaller image size
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy central package management files FIRST (required for version resolution)
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project files for dependency restoration
# Fleet service layers
COPY ["Services/Fleet/OrangeCarRental.Fleet.Api/OrangeCarRental.Fleet.Api.csproj", "Services/Fleet/OrangeCarRental.Fleet.Api/"]
COPY ["Services/Fleet/OrangeCarRental.Fleet.Application/OrangeCarRental.Fleet.Application.csproj", "Services/Fleet/OrangeCarRental.Fleet.Application/"]
COPY ["Services/Fleet/OrangeCarRental.Fleet.Domain/OrangeCarRental.Fleet.Domain.csproj", "Services/Fleet/OrangeCarRental.Fleet.Domain/"]
COPY ["Services/Fleet/OrangeCarRental.Fleet.Infrastructure/OrangeCarRental.Fleet.Infrastructure.csproj", "Services/Fleet/OrangeCarRental.Fleet.Infrastructure/"]

# BuildingBlocks dependencies
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/OrangeCarRental.BuildingBlocks.Domain.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/OrangeCarRental.BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/"]
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.EventStore/OrangeCarRental.BuildingBlocks.EventStore.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.EventStore/"]

# Cross-service dependency (ARCHITECTURAL SMELL - should be removed in future)
# Fleet.Infrastructure references Reservations.Infrastructure - not ideal for microservices
# Need all Reservations layers because of BuildingBlocks dependencies
COPY ["Services/Reservations/OrangeCarRental.Reservations.Domain/OrangeCarRental.Reservations.Domain.csproj", "Services/Reservations/OrangeCarRental.Reservations.Domain/"]
COPY ["Services/Reservations/OrangeCarRental.Reservations.Application/OrangeCarRental.Reservations.Application.csproj", "Services/Reservations/OrangeCarRental.Reservations.Application/"]
COPY ["Services/Reservations/OrangeCarRental.Reservations.Infrastructure/OrangeCarRental.Reservations.Infrastructure.csproj", "Services/Reservations/OrangeCarRental.Reservations.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Services/Fleet/OrangeCarRental.Fleet.Api/OrangeCarRental.Fleet.Api.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/Services/Fleet/OrangeCarRental.Fleet.Api"
RUN dotnet build "OrangeCarRental.Fleet.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "OrangeCarRental.Fleet.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage - Alpine for smaller image size (~100MB vs ~220MB)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1001 appuser && \
    adduser --system --uid 1001 --ingroup appuser appuser

# Copy published app
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "OrangeCarRental.Fleet.Api.dll"]
