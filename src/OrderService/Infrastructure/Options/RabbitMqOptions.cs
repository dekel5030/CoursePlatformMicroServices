namespace Infrastructure.Options;

public class RabbitMqOptions
{
    public static readonly string SectionName = "RabbitMq";

    public required string Host { get; set; }
    public string VirtualHost { get; set; } = "/";
    public required string Username { get; set; }
    public required string Password { get; set; }
}