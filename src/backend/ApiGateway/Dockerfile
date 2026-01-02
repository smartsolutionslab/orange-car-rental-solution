# Build stage - Alpine for smaller image size
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy central package management files FIRST (required for version resolution)
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy solution and project files
COPY ["ApiGateway/OrangeCarRental.ApiGateway/OrangeCarRental.ApiGateway.csproj", "ApiGateway/OrangeCarRental.ApiGateway/"]

# Restore dependencies
RUN dotnet restore "ApiGateway/OrangeCarRental.ApiGateway/OrangeCarRental.ApiGateway.csproj"

# Copy source code
COPY . .

# Build (disable analyzers due to RouteHandlerAnalyzer bug with C# 14 extension members)
WORKDIR "/src/ApiGateway/OrangeCarRental.ApiGateway"
RUN dotnet build "OrangeCarRental.ApiGateway.csproj" -c Release -o /app/build -p:RunAnalyzers=false

# Publish
FROM build AS publish
RUN dotnet publish "OrangeCarRental.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false -p:RunAnalyzers=false

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

ENTRYPOINT ["dotnet", "OrangeCarRental.ApiGateway.dll"]
