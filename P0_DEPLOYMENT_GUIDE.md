# P0 Deployment Guide - YANEDGE Email Management System

This guide covers the P0 (must-complete before launch) deployment steps for the YANEDGE Email Management System.

## Prerequisites

- .NET 10.0 SDK or later
- PostgreSQL 16 or later
- Redis 7 or later
- Docker and Docker Compose (optional, recommended)

## Quick Start with Docker Compose

The fastest way to get started is using Docker Compose, which will set up PostgreSQL, Redis, and the application:

```bash
# Start all services
docker-compose up -d

# Check service health
docker-compose ps

# View logs
docker-compose logs -f app
```

The application will be available at `http://localhost:5000`.

## Manual Setup

### 1. Configure External Dependencies

#### Option A: Using Docker Compose (Recommended)

Start PostgreSQL and Redis only:

```bash
# Start PostgreSQL and Redis
docker-compose up -d postgres redis

# Verify services are running
docker-compose ps
```

#### Option B: Manual Installation

Install and configure PostgreSQL and Redis on your system:

**PostgreSQL:**
- Host: localhost
- Port: 5432
- Database: EmailManagement
- Username: postgres
- Password: postgres (change in production!)

**Redis:**
- Host: localhost
- Port: 6379
- Password: (optional for development)

### 2. Update Configuration

Update the connection strings in `src/YANEDGE.EmailManagement.HttpApi.Host/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=EmailManagement;Username=postgres;Password=postgres"
  },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "EmailManagement:"
  }
}
```

For production, use `appsettings.Production.json` and secure your passwords!

### 3. Execute Database Migration

Run the migration script:

```bash
# Using the provided script
./scripts/run-migration.sh

# Or manually
dotnet ef database update \
    --project src/YANEDGE.EmailManagement.EntityFrameworkCore \
    --startup-project src/YANEDGE.EmailManagement.HttpApi.Host
```

**What the migration does:**
- Creates all database tables (MailAccounts, MailMessages, MailTemplates, MailRules, etc.)
- Sets up indexes for performance
- Configures foreign key relationships
- Initializes the migration history

### 4. Verify the Setup

#### Check Database Schema

Connect to PostgreSQL and verify:

```sql
-- List all tables
\dt

-- Check migration history
SELECT * FROM "__EFMigrationsHistory";

-- Verify key tables exist
SELECT COUNT(*) FROM "MailAccounts";
SELECT COUNT(*) FROM "MailMessages";
```

#### Test Redis Connection

```bash
# Using redis-cli
redis-cli ping
# Should return: PONG
```

### 5. Build and Run the Application

```bash
# Build the solution
dotnet build

# Run the application
cd src/YANEDGE.EmailManagement.HttpApi.Host
dotnet run
```

The application will start on:
- HTTP: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`
- Hangfire Dashboard: `http://localhost:5000/hangfire`
- Health Check: `http://localhost:5000/health`

## P0 Completion Checklist

- [x] MailSyncService implementation completed with real IMAP integration
- [x] Database migration script created and tested
- [x] PostgreSQL configuration documented
- [x] Redis configuration documented
- [x] Docker Compose setup for all dependencies
- [x] Build verification passed
- [ ] Execute database migration (requires running PostgreSQL)
- [ ] Verify application starts successfully
- [ ] Test mail sync functionality with a real email account

## Background Jobs Configured

The following Hangfire background jobs are automatically scheduled:

1. **Mail Sync Job** - Runs every 5 minutes
   - Syncs emails from all enabled IMAP accounts

2. **Send Task Processor** - Runs every 1 minute
   - Processes pending email sending tasks

3. **Rule Execution Job** - Runs every 10 minutes
   - Applies mail rules to new messages

4. **Failed Task Retry** - Runs every 30 minutes
   - Retries failed email operations

## Security Notes for Production

1. **Change Default Passwords**: Update PostgreSQL and Redis passwords
2. **Enable SSL/TLS**: Configure SSL for database and Redis connections
3. **Update CORS Origins**: Configure allowed origins in appsettings.Production.json
4. **Enable HTTPS**: Configure HTTPS with valid certificates
5. **Secure Hangfire Dashboard**: Restrict access to admin users only
6. **Encryption Keys**: Set secure encryption keys for password storage

## Troubleshooting

### Database Connection Issues

```bash
# Test PostgreSQL connection
psql -h localhost -U postgres -d EmailManagement

# Check if PostgreSQL is running
docker-compose ps postgres
```

### Redis Connection Issues

```bash
# Test Redis connection
redis-cli -h localhost -p 6379 ping

# Check if Redis is running
docker-compose ps redis
```

### Migration Errors

If migration fails:
1. Check database credentials
2. Ensure PostgreSQL is running and accessible
3. Verify the database user has CREATE TABLE permissions
4. Check logs for specific error messages

## Next Steps

After completing P0 deployment:
1. Configure at least one email account via API
2. Test email synchronization
3. Verify Hangfire jobs are running
4. Set up monitoring and alerting
5. Configure backup strategy for PostgreSQL

## Support

For issues or questions, please refer to:
- Architecture Documentation: `/01-architecture/`
- API Documentation: `/02-integration/05-api-design.md`
- Deployment Guide: `/03-governance/06-permission-security-devops-spec.md`
