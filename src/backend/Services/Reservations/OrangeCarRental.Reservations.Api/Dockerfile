FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy Directory.Build.props and Directory.Packages.props for central package management
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy project files for restore
COPY BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/OrangeCarRental.BuildingBlocks.Domain.csproj BuildingBlocks/OrangeCarRental.BuildingBlocks.Domain/
COPY BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/OrangeCarRental.BuildingBlocks.Infrastructure.csproj BuildingBlocks/OrangeCarRental.BuildingBlocks.Infrastructure/
COPY BuildingBlocks/OrangeCarRental.BuildingBlocks.EventStore/OrangeCarRental.BuildingBlocks.EventStore.csproj BuildingBlocks/OrangeCarRental.BuildingBlocks.EventStore/
COPY ServiceDefaults/OrangeCarRental.ServiceDefaults.csproj ServiceDefaults/
COPY Services/Reservations/OrangeCarRental.Reservations.Domain/OrangeCarRental.Reservations.Domain.csproj Services/Reservations/OrangeCarRental.Reservations.Domain/
COPY Services/Reservations/OrangeCarRental.Reservations.Application/OrangeCarRental.Reservations.Application.csproj Services/Reservations/OrangeCarRental.Reservations.Application/
COPY Services/Reservations/OrangeCarRental.Reservations.Infrastructure/OrangeCarRental.Reservations.Infrastructure.csproj Services/Reservations/OrangeCarRental.Reservations.Infrastructure/
COPY Services/Reservations/OrangeCarRental.Reservations.Api/OrangeCarRental.Reservations.Api.csproj Services/Reservations/OrangeCarRental.Reservations.Api/
COPY Services/Customers/OrangeCarRental.Customers.Domain/OrangeCarRental.Customers.Domain.csproj Services/Customers/OrangeCarRental.Customers.Domain/

# Restore dependencies
RUN dotnet restore Services/Reservations/OrangeCarRental.Reservations.Api/OrangeCarRental.Reservations.Api.csproj

# Copy source code
COPY . .

# Build
WORKDIR /src/Services/Reservations/OrangeCarRental.Reservations.Api
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrangeCarRental.Reservations.Api.dll"]
