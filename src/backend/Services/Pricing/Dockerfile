# Build stage - Alpine for smaller image size
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy central package management files FIRST (required for version resolution)
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy project files for dependency restoration
# Pricing service layers
COPY ["Services/Pricing/OrangeCarRental.Pricing.Api/OrangeCarRental.Pricing.Api.csproj", "Services/Pricing/OrangeCarRental.Pricing.Api/"]
COPY ["Services/Pricing/OrangeCarRental.Pricing.Application/OrangeCarRental.Pricing.Application.csproj", "Services/Pricing/OrangeCarRental.Pricing.Application/"]
COPY ["Services/Pricing/OrangeCarRental.Pricing.Domain/OrangeCarRental.Pricing.Domain.csproj", "Services/Pricing/OrangeCarRental.Pricing.Domain/"]
COPY ["Services/Pricing/OrangeCarRental.Pricing.Infrastructure/OrangeCarRental.Pricing.Infrastructure.csproj", "Services/Pricing/OrangeCarRental.Pricing.Infrastructure/"]

# BuildingBlocks dependencies
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/OrangeCarRental.BuildingBlocks.Domain.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/"]
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/OrangeCarRental.BuildingBlocks.Infrastructure.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/"]
COPY ["BuildingBlocks/OrangeCarRental.BuildingBlocks.EventStore/OrangeCarRental.BuildingBlocks.EventStore.csproj", "BuildingBlocks/OrangeCarRental.BuildingBlocks.EventStore/"]

# Restore dependencies
RUN dotnet restore "Services/Pricing/OrangeCarRental.Pricing.Api/OrangeCarRental.Pricing.Api.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/Services/Pricing/OrangeCarRental.Pricing.Api"
RUN dotnet build "OrangeCarRental.Pricing.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "OrangeCarRental.Pricing.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage - Alpine for smaller image size (~100MB vs ~220MB)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
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

ENTRYPOINT ["dotnet", "OrangeCarRental.Pricing.Api.dll"]
