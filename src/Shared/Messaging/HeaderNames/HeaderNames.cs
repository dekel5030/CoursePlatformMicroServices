namespace Common.Messaging;

public static class HeaderNames
{
    public const string CorrelationId = "correlation-id";
    public const string CausationId   = "causation-id";
    public const string TraceParent   = "aceparent";
    public const string TraceState    = "acestate";
    public const string EventId       = "event-id";
    public const string EventType     = "event-type";
    public const string Version       = "version";
    public const string AggregateType = "aggregate-type";
    public const string AggregateId   = "aggregate-id";
    public const string TenantId      = "tenant-id";
}
