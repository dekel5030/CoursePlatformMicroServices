using System.ComponentModel.DataAnnotations;

namespace Common.Messaging.Options;

public sealed class RabbitMqOptions
{
    public const string SectionName = "Rabbit";

    [Required]
    public string Host { get; init; } = "localhost";
    public string VirtualHost { get; init; } = "/";

    [Required]
    public string User { get; init; } = "guest";

    [Required]
    public string Password { get; init; } = "guest";

    [Range(1, 65535)]
    public int Port { get; init; } = 5672;

    public bool UseSsl { get; init; } = false;

    public Uri CreateUri()
    {
        var vhost = string.IsNullOrWhiteSpace(VirtualHost)
            ? "/"
            : (VirtualHost.StartsWith("/") ? VirtualHost : "/" + VirtualHost);

        var scheme = UseSsl ? "amqps" : "amqp";

        return new UriBuilder
        {
            Scheme = scheme,
            Host   = Host,
            Port   = Port,
            Path   = vhost
        }.Uri;
    }
}