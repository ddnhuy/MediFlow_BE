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
COPY ["src/Services/VaccinationReception/VaccinationReception.API/VaccinationReception.API.csproj", "src/Services/VaccinationReception/VaccinationReception.API/"]
COPY ["src/BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/BuildingBlocks/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Strings/BuildingBlocks.Strings.csproj", "src/BuildingBlocks/BuildingBlocks.Strings/"]
COPY ["src/Services/VaccinationReception/VaccinationReception.Application/VaccinationReception.Application.csproj", "src/Services/VaccinationReception/VaccinationReception.Application/"]
COPY ["src/Services/VaccinationReception/VaccinationReception.Infrastructure/VaccinationReception.Infrastructure.csproj", "src/Services/VaccinationReception/VaccinationReception.Infrastructure/"]
COPY ["src/Services/VaccinationReception/VaccinationReception.Domain/VaccinationReception.Domain.csproj", "src/Services/VaccinationReception/VaccinationReception.Domain/"]
RUN dotnet restore "./src/Services/VaccinationReception/VaccinationReception.API/VaccinationReception.API.csproj"
COPY . .
WORKDIR "/src/src/Services/VaccinationReception/VaccinationReception.API"
RUN dotnet build "./VaccinationReception.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VaccinationReception.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VaccinationReception.API.dll"]