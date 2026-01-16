# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Driver.Services.Api/Driver.Services.Api.csproj", "Driver.Services.Api/"]
COPY ["Driver.Services.Application/Driver.Services.Application.csproj", "Driver.Services.Application/"]
COPY ["Driver.Services.Domain/Driver.Services.Domain.csproj", "Driver.Services.Domain/"]
COPY ["Driver.Services.Infrastructure/Driver.Services.Infrastructure.csproj", "Driver.Services.Infrastructure/"]

RUN dotnet restore "Driver.Services.Api/Driver.Services.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Driver.Services.Api"
RUN dotnet build "Driver.Services.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Driver.Services.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=15s --timeout=5s --start-period=30s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Driver.Services.Api.dll"]
