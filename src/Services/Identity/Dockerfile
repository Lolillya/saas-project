# 1. Use the ASP.NET runtime for the final image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Use the SDK to build the code
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 3. Copy the .csproj files first (for better caching)
COPY ["src/Services/Identity/Identity.Api/Identity.Api.csproj", "Services/Identity/Identity.Api/"]
COPY ["src/Services/Identity/Identity.Application/Identity.Application.csproj", "Services/Identity/Identity.Application/"]
COPY ["src/Services/Identity/Identity.Domain/Identity.Domain.csproj", "Services/Identity/Identity.Domain/"]
COPY ["src/Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Services/Identity/Identity.Infrastructure/"]
COPY ["src/Shared/Shared.Kernel/Shared.Kernel.csproj", "Shared/Shared.Kernel/"]
COPY ["src/Shared/Shared.MultiTenancy/Shared.MultiTenancy.csproj", "Shared/Shared.MultiTenancy/"]

# 4. Restore dependencies
RUN dotnet restore "Services/Identity/Identity.Api/Identity.Api.csproj"

# 5. Copy the rest of the source code and build
COPY src/ .
WORKDIR "/src/Services/Identity/Identity.Api"
RUN dotnet build "Identity.Api.csproj" -c Release -o /app/build

# 6. Publish the app
FROM build AS publish
RUN dotnet publish "Identity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 7. Final stage: copy the published files to the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]