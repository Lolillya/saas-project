FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/ApiGateway/InventorySaas.Gateway/InventorySaas.Gateway.csproj", "ApiGateway/InventorySaas.Gateway/"]

RUN dotnet restore "ApiGateway/InventorySaas.Gateway/InventorySaas.Gateway.csproj"

COPY src/ .
WORKDIR "/src/ApiGateway/InventorySaas.Gateway"

FROM build AS publish
RUN dotnet publish "InventorySaas.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventorySaas.Gateway.dll"]