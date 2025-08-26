using System.ComponentModel.DataAnnotations;

namespace Common.Messaging.Options;

public sealed class MassTransitOptions
{
    public const string SectionName = "MassTransit";

    public bool UseKebabCaseEndpoints { get; init; } = true;
    public ushort? PrefetchCount { get; init; } = null;

    [Range(1, 100)]
    public int RetryCount { get; init; } = 5;

    [Range(1, 60)]
    public int RetryInitialSeconds { get; init; } = 1;

    [Range(1, 60)]
    public int RetryIncrementSeconds { get; init; } = 2;
}