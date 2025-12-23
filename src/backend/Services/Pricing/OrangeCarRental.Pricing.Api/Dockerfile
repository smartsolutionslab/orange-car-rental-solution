FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy Directory.Build.props and Directory.Packages.props for central package management
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy project files for restore
COPY BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/OrangeCarRental.BuildingBlocks.Domain.csproj BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/
COPY BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/OrangeCarRental.BuildingBlocks.Infrastructure.csproj BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/
COPY ServiceDefaults/OrangeCarRental.ServiceDefaults.csproj ServiceDefaults/
COPY Services/Pricing/OrangeCarRental.Pricing.Domain/OrangeCarRental.Pricing.Domain.csproj Services/Pricing/OrangeCarRental.Pricing.Domain/
COPY Services/Pricing/OrangeCarRental.Pricing.Application/OrangeCarRental.Pricing.Application.csproj Services/Pricing/OrangeCarRental.Pricing.Application/
COPY Services/Pricing/OrangeCarRental.Pricing.Infrastructure/OrangeCarRental.Pricing.Infrastructure.csproj Services/Pricing/OrangeCarRental.Pricing.Infrastructure/
COPY Services/Pricing/OrangeCarRental.Pricing.Api/OrangeCarRental.Pricing.Api.csproj Services/Pricing/OrangeCarRental.Pricing.Api/

# Restore dependencies
RUN dotnet restore Services/Pricing/OrangeCarRental.Pricing.Api/OrangeCarRental.Pricing.Api.csproj

# Copy source code
COPY . .

# Build
WORKDIR /src/Services/Pricing/OrangeCarRental.Pricing.Api
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrangeCarRental.Pricing.Api.dll"]
