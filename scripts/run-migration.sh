#!/bin/bash
# Database Migration Script for YANEDGE Email Management System
# This script executes EF Core migrations to update the database schema

set -e

echo "======================================"
echo "YANEDGE Email Management - Database Migration"
echo "======================================"
echo ""

# Check if .NET EF tool is installed
if ! command -v dotnet-ef &> /dev/null; then
    echo "Installing dotnet-ef tool..."
    dotnet tool install --global dotnet-ef
fi

# Navigate to solution root
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR/.."

echo "Checking database connection..."
echo ""

# Execute migration
echo "Executing database migration..."
dotnet ef database update \
    --project src/YANEDGE.EmailManagement.EntityFrameworkCore \
    --startup-project src/YANEDGE.EmailManagement.HttpApi.Host

if [ $? -eq 0 ]; then
    echo ""
    echo "✓ Database migration completed successfully!"
    echo ""
    echo "Next steps:"
    echo "1. Verify database schema in PostgreSQL"
    echo "2. Check migration history: SELECT * FROM __EFMigrationsHistory;"
    echo "3. Start the application: dotnet run --project src/YANEDGE.EmailManagement.HttpApi.Host"
else
    echo ""
    echo "✗ Database migration failed!"
    echo ""
    echo "Troubleshooting:"
    echo "1. Ensure PostgreSQL is running"
    echo "2. Verify connection string in appsettings.json"
    echo "3. Check PostgreSQL credentials"
    echo "4. Ensure database exists or has CREATE DATABASE permission"
    exit 1
fi
