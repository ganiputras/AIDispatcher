using System.Diagnostics;

namespace AIDispatcher.Observability;

/// <summary>
/// Global ActivitySource used for tracing AIDispatcher activities via OpenTelemetry.
/// </summary>
public static class DispatcherActivitySource
{
    /// <summary>
    /// The shared activity source name. Can be configured in observability dashboards (e.g., Jaeger, Zipkin).
    /// </summary>
    public const string Name = "AIDispatcher";

    /// <summary>
    /// The global ActivitySource instance used for tracing across dispatcher pipeline.
    /// </summary>
    public static readonly ActivitySource Instance = new(Name);
}