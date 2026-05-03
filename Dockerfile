# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["YANEDGE.EmailManagement.slnx", "./"]
COPY ["src/YANEDGE.EmailManagement.Domain.Shared/YANEDGE.EmailManagement.Domain.Shared.csproj", "src/YANEDGE.EmailManagement.Domain.Shared/"]
COPY ["src/YANEDGE.EmailManagement.Domain/YANEDGE.EmailManagement.Domain.csproj", "src/YANEDGE.EmailManagement.Domain/"]
COPY ["src/YANEDGE.EmailManagement.Application.Contracts/YANEDGE.EmailManagement.Application.Contracts.csproj", "src/YANEDGE.EmailManagement.Application.Contracts/"]
COPY ["src/YANEDGE.EmailManagement.Application/YANEDGE.EmailManagement.Application.csproj", "src/YANEDGE.EmailManagement.Application/"]
COPY ["src/YANEDGE.EmailManagement.EntityFrameworkCore/YANEDGE.EmailManagement.EntityFrameworkCore.csproj", "src/YANEDGE.EmailManagement.EntityFrameworkCore/"]
COPY ["src/YANEDGE.EmailManagement.HttpApi/YANEDGE.EmailManagement.HttpApi.csproj", "src/YANEDGE.EmailManagement.HttpApi/"]
COPY ["src/YANEDGE.EmailManagement.HttpApi.Host/YANEDGE.EmailManagement.HttpApi.Host.csproj", "src/YANEDGE.EmailManagement.HttpApi.Host/"]

# Restore dependencies
RUN dotnet restore "src/YANEDGE.EmailManagement.HttpApi.Host/YANEDGE.EmailManagement.HttpApi.Host.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/src/YANEDGE.EmailManagement.HttpApi.Host"
RUN dotnet build "YANEDGE.EmailManagement.HttpApi.Host.csproj" -c Release -o /app/build
RUN dotnet publish "YANEDGE.EmailManagement.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install PostgreSQL client for health checks
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=build /app/publish .

# Create non-root user for security
RUN useradd -m -s /bin/bash appuser && chown -R appuser:appuser /app
USER appuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health/live || exit 1

# Start application
ENTRYPOINT ["dotnet", "YANEDGE.EmailManagement.HttpApi.Host.dll"]
