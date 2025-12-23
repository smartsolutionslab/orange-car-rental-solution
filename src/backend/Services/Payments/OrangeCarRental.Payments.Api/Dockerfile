FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy Directory.Build.props, Directory.Packages.props, and NuGet.config for central package management
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY NuGet.config ./

# Copy project files for restore
COPY BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/OrangeCarRental.BuildingBlocks.Domain.csproj BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/
COPY BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/OrangeCarRental.BuildingBlocks.Infrastructure.csproj BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/
COPY ServiceDefaults/OrangeCarRental.ServiceDefaults.csproj ServiceDefaults/
COPY Services/Payments/OrangeCarRental.Payments.Domain/OrangeCarRental.Payments.Domain.csproj Services/Payments/OrangeCarRental.Payments.Domain/
COPY Services/Payments/OrangeCarRental.Payments.Application/OrangeCarRental.Payments.Application.csproj Services/Payments/OrangeCarRental.Payments.Application/
COPY Services/Payments/OrangeCarRental.Payments.Infrastructure/OrangeCarRental.Payments.Infrastructure.csproj Services/Payments/OrangeCarRental.Payments.Infrastructure/
COPY Services/Payments/OrangeCarRental.Payments.Api/OrangeCarRental.Payments.Api.csproj Services/Payments/OrangeCarRental.Payments.Api/

# Restore dependencies
RUN dotnet restore Services/Payments/OrangeCarRental.Payments.Api/OrangeCarRental.Payments.Api.csproj

# Copy source code
COPY . .

# Build
WORKDIR /src/Services/Payments/OrangeCarRental.Payments.Api
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrangeCarRental.Payments.Api.dll"]
