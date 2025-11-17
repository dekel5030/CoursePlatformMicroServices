# Database Migrations

This folder contains SQL migration scripts for the AuthService database.

## How to Apply Migrations

### Using psql (PostgreSQL command-line tool)

```bash
psql -h localhost -p 5433 -U dekel -d authdb -f 001_AddRefreshTokenToAuthUser.sql
```

### Using Entity Framework Core Migrations

If you prefer to use EF Core migrations instead of manual SQL scripts:

```bash
cd src/AuthService/Infrastructure
dotnet ef migrations add AddRefreshTokenToAuthUser --startup-project ../Web.Api
dotnet ef database update --startup-project ../Web.Api
```

## Migration Order

1. `001_AddRefreshTokenToAuthUser.sql` - Adds refresh token support to AuthUsers table

## Notes

- The refresh_token column is nullable to support existing users
- An index is created on refresh_token for faster lookups
- The migration uses snake_case naming convention to match PostgreSQL conventions
