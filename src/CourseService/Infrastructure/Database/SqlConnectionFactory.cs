using System.Data;
using Courses.Application.Abstractions.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Courses.Infrastructure.Database;

internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private const string ReadDatabaseConnectionSection = "ReadDatabase";
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString(ReadDatabaseConnectionSection)
            ?? throw new InvalidOperationException("Read database connection string not found");
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
