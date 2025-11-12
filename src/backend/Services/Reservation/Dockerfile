# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy central package management files FIRST (required for version resolution)
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project files for dependency restoration
# Reservations service layers
COPY ["Services/Reservations/OrangeCarRental.Reservations.Api/OrangeCarRental.Reservations.Api.csproj", "Services/Reservations/OrangeCarRental.Reservations.Api/"]
COPY ["Services/Reservations/OrangeCarRental.Reservations.Application/OrangeCarRental.Reservations.Application.csproj", "Services/Reservations/OrangeCarRental.Reservations.Application/"]
COPY ["Services/Reservations/OrangeCarRental.Reservations.Domain/OrangeCarRental.Reservations.Domain.csproj", "Services/Reservations/OrangeCarRental.Reservations.Domain/"]
COPY ["Services/Reservations/OrangeCarRental.Reservations.Infrastructure/OrangeCarRental.Reservations.Infrastructure.csproj", "Services/Reservations/OrangeCarRental.Reservations.Infrastructure/"]

# BuildingBlocks dependencies
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/OrangeCarRental.BuildingBlocks.Domain.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/OrangeCarRental.BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/"]
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.EventStore/OrangeCarRental.BuildingBlocks.EventStore.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.EventStore/"]

# Restore dependencies
RUN dotnet restore "Services/Reservations/OrangeCarRental.Reservations.Api/OrangeCarRental.Reservations.Api.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/Services/Reservations/OrangeCarRental.Reservations.Api"
RUN dotnet build "OrangeCarRental.Reservations.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "OrangeCarRental.Reservations.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
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

ENTRYPOINT ["dotnet", "OrangeCarRental.Reservations.Api.dll"]
