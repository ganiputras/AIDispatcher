using System.Diagnostics;

namespace AIDispatcher.Observability
{
    /// <summary>
    /// ActivitySource global yang digunakan untuk tracing aktivitas AIDispatcher melalui OpenTelemetry.
    /// </summary>
    public static class DispatcherActivitySource
    {
        /// <summary>
        /// Nama sumber aktivitas (ActivitySource) yang digunakan bersama.
        /// Nama ini dapat dikonfigurasi pada dashboard observability seperti Jaeger atau Zipkin.
        /// </summary>
        public const string Name = "AIDispatcher";

        /// <summary>
        /// Instance global ActivitySource yang digunakan untuk tracing di seluruh pipeline dispatcher.
        /// </summary>
        public static readonly ActivitySource Instance = new(Name);
    }
}