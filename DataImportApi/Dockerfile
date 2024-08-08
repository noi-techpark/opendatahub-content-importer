#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Helper/Helper.csproj", "Helper/"]
COPY ["LTSAPI/LTSAPI.csproj", "LTSAPI/"]
COPY ["HGVApi/HGVApi.csproj", "HGVApi/"]
COPY ["RabbitPusher/RabbitPusher.csproj", "RabbitPusher/"]
COPY ["DataImportHelper/DataImportHelper.csproj", "DataImportHelper/"]
COPY ["DataImportApi/DataImportApi.csproj", "DataImportApi/"]
RUN dotnet restore "./DataImportApi/DataImportApi.csproj"
COPY . .
WORKDIR "/src/DataImportApi"
RUN dotnet build "./DataImportApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DataImportApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY DataImportApi/appsettings.Production.json appsettings.json

# Expose the API port
EXPOSE 80

ENTRYPOINT ["dotnet", "DataImportApi.dll"]