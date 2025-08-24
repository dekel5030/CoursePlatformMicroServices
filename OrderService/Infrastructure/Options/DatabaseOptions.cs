using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class DatabaseOptions
{
    public static readonly string SectionName = "OrdersDb";

    [Required]
    public string Host { get; set; } = string.Empty;

    [Required]
    public string Database { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string BuildConnectionString()
    {
        var builder = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = Database,
            Username = Username,
            Password = Password
        };

        return builder.ConnectionString;
    }
}