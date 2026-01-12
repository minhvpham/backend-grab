# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Driver.Services.Api/Driver.Services.Api.csproj", "Driver.Services.Api/"]
COPY ["Driver.Services.Application/Driver.Services.Application.csproj", "Driver.Services.Application/"]
COPY ["Driver.Services.Domain/Driver.Services.Domain.csproj", "Driver.Services.Domain/"]
COPY ["Driver.Services.Infrastructure/Driver.Services.Infrastructure.csproj", "Driver.Services.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Driver.Services.Api/Driver.Services.Api.csproj"

# Copy everything else
COPY . .

# Build the application
WORKDIR "/src/Driver.Services.Api"
RUN dotnet build "Driver.Services.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Driver.Services.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Install EF Core tools for migrations
RUN dotnet tool install --global dotnet-ef --version 8.0.11
ENV PATH="${PATH}:/root/.dotnet/tools"

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install dotnet-ef tool in runtime image
COPY --from=mcr.microsoft.com/dotnet/sdk:8.0 /usr/share/dotnet /usr/share/dotnet
RUN dotnet tool install --global dotnet-ef --version 8.0.11
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy published app
COPY --from=publish /app/publish .

# Copy the entire source for migrations
COPY --from=build /src /src

# Create entrypoint script for automatic migration
RUN echo '#!/bin/bash\n\
set -e\n\
echo "Waiting for database to be ready..."\n\
until dotnet ef database update --project /src/Driver.Services.Infrastructure --startup-project /src/Driver.Services.Api --no-build 2>&1 | grep -q "Done" || dotnet ef database update --project /src/Driver.Services.Infrastructure --startup-project /src/Driver.Services.Api --no-build 2>&1 | grep -q "No migrations"; do\n\
  echo "Retrying migration..."\n\
  sleep 2\n\
done\n\
echo "Database migration completed successfully!"\n\
echo "Starting application..."\n\
exec dotnet Driver.Services.Api.dll' > /app/entrypoint.sh \
    && chmod +x /app/entrypoint.sh

EXPOSE 8080

ENTRYPOINT ["/app/entrypoint.sh"]
