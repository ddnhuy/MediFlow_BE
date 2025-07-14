#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Authentication/Authentication.API/Authentication.API.csproj", "src/Services/Authentication/Authentication.API/"]
COPY ["src/Services/Authentication/Authentication.Business/Authentication.Business.csproj", "src/Services/Authentication/Authentication.Business/"]
COPY ["src/BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/BuildingBlocks/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Strings/BuildingBlocks.Strings.csproj", "src/BuildingBlocks/BuildingBlocks.Strings/"]
COPY ["src/Services/Authentication/Authentication.DataAccess/Authentication.DataAccess.csproj", "src/Services/Authentication/Authentication.DataAccess/"]
RUN dotnet restore "./src/Services/Authentication/Authentication.API/Authentication.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Authentication/Authentication.API"
RUN dotnet build "./Authentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Authentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.API.dll"]