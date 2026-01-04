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
COPY ServiceDefaults/OrangeCarRental.ServiceDefaults.csproj ServiceDefaults/
COPY Services/Customers/OrangeCarRental.Customers.Domain/OrangeCarRental.Customers.Domain.csproj Services/Customers/OrangeCarRental.Customers.Domain/
COPY Services/Customers/OrangeCarRental.Customers.Application/OrangeCarRental.Customers.Application.csproj Services/Customers/OrangeCarRental.Customers.Application/
COPY Services/Customers/OrangeCarRental.Customers.Infrastructure/OrangeCarRental.Customers.Infrastructure.csproj Services/Customers/OrangeCarRental.Customers.Infrastructure/
COPY Services/Customers/OrangeCarRental.Customers.Api/OrangeCarRental.Customers.Api.csproj Services/Customers/OrangeCarRental.Customers.Api/

# Restore dependencies
RUN dotnet restore Services/Customers/OrangeCarRental.Customers.Api/OrangeCarRental.Customers.Api.csproj

# Copy source code
COPY . .

# Build
WORKDIR /src/Services/Customers/OrangeCarRental.Customers.Api
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrangeCarRental.Customers.Api.dll"]
