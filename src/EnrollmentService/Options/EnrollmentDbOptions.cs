using System.ComponentModel.DataAnnotations;

namespace EnrollmentService.Options;

public class EnrollmentDbOptions
{
    public const string SectionName = "EnrollmentDb";

    [Required(ErrorMessage = $"{nameof(EnrollmentDbOptions)}: Host is required.")]
    public required string Host { get; set; }
    [Required(ErrorMessage = $"{nameof(EnrollmentDbOptions)}: Port is required.")]
    public required int Port { get; set; }

    [Required(ErrorMessage = $"{nameof(EnrollmentDbOptions)}: Database is required.")]
    public required string Database { get; set; }

    [Required(ErrorMessage = $"{nameof(EnrollmentDbOptions)}: Username is required.")]
    public required string Username { get; set; }

    [Required(ErrorMessage = $"{nameof(EnrollmentDbOptions)}: Password is required.")]
    public required string Password { get; set; }

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
