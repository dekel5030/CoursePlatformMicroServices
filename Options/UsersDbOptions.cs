using System.ComponentModel.DataAnnotations;

namespace UserService.Options;

public sealed class UsersDbOptions
{
    public const string SectionName = "UsersDb";

    [Required(ErrorMessage = "UsersDbOptions: database host is required.")]
    public required string Host { get; set; } 

    [Range(1, 65535, ErrorMessage = "UsersDbOptions: database port must be between 1 and 65535.")]
    public int Port { get; set; }

    [Required(ErrorMessage = "UsersDbOptions: database name is required.")]
    public required string Database { get; set; }

    [Required(ErrorMessage = "UsersDbOptions: username is required.")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "UsersDbOptions: password is required.")]
    public required string Password { get; set; }

    public string BuildConnectionString()
    {
        var ConnectionStringBuilder = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = Database,
            Username = Username,
            Password = Password
        };

        return ConnectionStringBuilder.ToString();
    }
}