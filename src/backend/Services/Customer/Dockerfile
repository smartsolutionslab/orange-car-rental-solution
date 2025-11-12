# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy central package management files FIRST (required for version resolution)
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project files for dependency restoration
# Customers service layers
COPY ["Services/Customers/OrangeCarRental.Customers.Api/OrangeCarRental.Customers.Api.csproj", "Services/Customers/OrangeCarRental.Customers.Api/"]
COPY ["Services/Customers/OrangeCarRental.Customers.Application/OrangeCarRental.Customers.Application.csproj", "Services/Customers/OrangeCarRental.Customers.Application/"]
COPY ["Services/Customers/OrangeCarRental.Customers.Domain/OrangeCarRental.Customers.Domain.csproj", "Services/Customers/OrangeCarRental.Customers.Domain/"]
COPY ["Services/Customers/OrangeCarRental.Customers.Infrastructure/OrangeCarRental.Customers.Infrastructure.csproj", "Services/Customers/OrangeCarRental.Customers.Infrastructure/"]

# BuildingBlocks dependencies
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/OrangeCarRental.BuildingBlocks.Domain.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/OrangeCarRental.BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/"]
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.EventStore/OrangeCarRental.BuildingBlocks.EventStore.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.EventStore/"]

# Restore dependencies
RUN dotnet restore "Services/Customers/OrangeCarRental.Customers.Api/OrangeCarRental.Customers.Api.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/Services/Customers/OrangeCarRental.Customers.Api"
RUN dotnet build "OrangeCarRental.Customers.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "OrangeCarRental.Customers.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

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

ENTRYPOINT ["dotnet", "OrangeCarRental.Customers.Api.dll"]
