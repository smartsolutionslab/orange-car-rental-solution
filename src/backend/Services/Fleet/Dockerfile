# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Services/Fleet/OrangeCarRental.FleetService/OrangeCarRental.FleetService.csproj", "Services/Fleet/OrangeCarRental.FleetService/"]

# Restore dependencies
RUN dotnet restore "Services/Fleet/OrangeCarRental.FleetService/OrangeCarRental.FleetService.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/Services/Fleet/OrangeCarRental.FleetService"
RUN dotnet build "OrangeCarRental.FleetService.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "OrangeCarRental.FleetService.csproj" -c Release -o /app/publish /p:UseAppHost=false

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

ENTRYPOINT ["dotnet", "OrangeCarRental.FleetService.dll"]
