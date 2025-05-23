# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Inventory/Inventory.API/Inventory.API.csproj", "src/Services/Inventory/Inventory.API/"]
COPY ["src/Services/Inventory/Inventory.Application/Inventory.Application.csproj", "src/Services/Inventory/Inventory.Application/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "src/BuildingBlocks/BuildingBlocks.Messaging/"]
COPY ["src/BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/BuildingBlocks/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Strings/BuildingBlocks.Strings.csproj", "src/BuildingBlocks/BuildingBlocks.Strings/"]
COPY ["src/Services/Inventory/Inventory.Domain/Inventory.Domain.csproj", "src/Services/Inventory/Inventory.Domain/"]
COPY ["src/Services/Inventory/Inventory.Infrastructure/Inventory.Infrastructure.csproj", "src/Services/Inventory/Inventory.Infrastructure/"]
RUN dotnet restore "./src/Services/Inventory/Inventory.API/Inventory.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Inventory/Inventory.API"
RUN dotnet build "./Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventory.API.dll"]