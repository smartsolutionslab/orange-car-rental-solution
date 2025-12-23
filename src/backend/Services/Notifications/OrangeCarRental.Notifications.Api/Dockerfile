FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy Directory.Build.props and Directory.Packages.props for central package management
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy project files for restore
COPY BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/OrangeCarRental.BuildingBlocks.Domain.csproj BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/
COPY BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/OrangeCarRental.BuildingBlocks.Infrastructure.csproj BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/
COPY ServiceDefaults/OrangeCarRental.ServiceDefaults.csproj ServiceDefaults/
COPY Services/Notifications/OrangeCarRental.Notifications.Domain/OrangeCarRental.Notifications.Domain.csproj Services/Notifications/OrangeCarRental.Notifications.Domain/
COPY Services/Notifications/OrangeCarRental.Notifications.Application/OrangeCarRental.Notifications.Application.csproj Services/Notifications/OrangeCarRental.Notifications.Application/
COPY Services/Notifications/OrangeCarRental.Notifications.Infrastructure/OrangeCarRental.Notifications.Infrastructure.csproj Services/Notifications/OrangeCarRental.Notifications.Infrastructure/
COPY Services/Notifications/OrangeCarRental.Notifications.Api/OrangeCarRental.Notifications.Api.csproj Services/Notifications/OrangeCarRental.Notifications.Api/

# Restore dependencies
RUN dotnet restore Services/Notifications/OrangeCarRental.Notifications.Api/OrangeCarRental.Notifications.Api.csproj

# Copy source code
COPY . .

# Build
WORKDIR /src/Services/Notifications/OrangeCarRental.Notifications.Api
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrangeCarRental.Notifications.Api.dll"]
