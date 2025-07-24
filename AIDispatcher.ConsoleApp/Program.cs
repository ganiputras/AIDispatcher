using AIDispatcher.Core;
using AIDispatcher.Core.Commons;
using AIDispatcher.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AIDispatcher.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();


        // ===========================================
        // Registrasi Fitur Utama AIDispatcher ke DI
        // ===========================================

        /// <summary>
        /// WAJIB. Registrasi handler dan pipeline inti (logging, exception, dsb).
        /// </summary>
        builder.Services.AddAIDispatcherCore();

        /// <summary>
        /// OPSIONAL. Registrasi pipeline lanjutan (retry, timeout, circuit breaker, performance, dsb).
        /// Aktifkan jika ingin fitur-fitur advanced.
        /// </summary>
        // builder.Services.AddAIDispatcherAdvanced();

        // ===========================================
        // Konfigurasi Global Opsi Dispatcher (Opsional)
        // ===========================================
        //builder.Services.Configure<DispatcherOptions>(opt =>
        //{
        //    opt.PublishStrategy = PublishStrategy.Parallel;      // Eksekusi notifikasi: Parallel/Sequential
        //    opt.PerformanceThresholdMs = 1000;                  // Warning jika eksekusi > 1000ms
        //});

        // ===========================================
        // Pipeline Behavior untuk Request/Command/Query
        // ===========================================

        // Logging pipeline - Catat waktu & hasil request/command/query
        // builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Exception pipeline - Tangkap & handle global error
        // builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));

        // Retry pipeline - Retry otomatis jika handler gagal (Polly)
        // builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));

        // Timeout pipeline - Batasi waktu eksekusi handler
        // builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TimeoutBehavior<,>));

        // Circuit breaker pipeline - Putus sementara handler error beruntun
        // builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CircuitBreakerBehavior<,>));

        // Performance pipeline - Warning/log jika eksekusi lambat
        // builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        // Pre/Post processor pipeline - Mendukung hook pre/post ala MediatR
        // builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PrePostProcessorBehavior<,>));


        // Validation pipeline - Validasi otomatis request (gunakan FluentValidation jika tersedia)
        // builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MyValidationBehavior<,>));

        // Caching pipeline - Cache hasil handler/query (jika tersedia)
        // builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MyCachingBehavior<,>));


        // ===========================================
        // Pipeline Behavior untuk Notification
        // ===========================================

        // Logging pipeline untuk notifikasi
        // builder.Services.AddScoped(typeof(INotificationPipelineBehavior<>), typeof(LoggingNotificationBehavior<>));

        // Exception pipeline untuk notifikasi
        // builder.Services.AddScoped(typeof(INotificationPipelineBehavior<>), typeof(NotificationExceptionBehavior<>));

        // Retry pipeline untuk notifikasi
        // builder.Services.AddScoped(typeof(INotificationPipelineBehavior<>), typeof(RetryNotificationBehavior<>));

        // Timeout pipeline untuk notifikasi
        // builder.Services.AddScoped(typeof(INotificationPipelineBehavior<>), typeof(NotificationTimeoutBehavior<>));

        // Circuit breaker pipeline untuk notifikasi
        // builder.Services.AddScoped(typeof(INotificationPipelineBehavior<>), typeof(CircuitBreakerNotificationBehavior<>));

        // Performance pipeline untuk notifikasi
        // builder.Services.AddScoped(typeof(INotificationPipelineBehavior<>), typeof(NotificationPerformanceBehavior<>));

        // ===========================================
        // Registrasi Pre/Post Processor (Opsional)
        // ===========================================

        // Pre-processor: kode dijalankan SEBELUM handler utama
        // builder.Services.AddScoped(typeof(IRequestPreProcessor<>), typeof(MyLoggingPreProcessor<>));

        // Post-processor: kode dijalankan SETELAH handler utama selesai
        // builder.Services.AddScoped(typeof(IRequestPostProcessor<,>), typeof(MyLoggingPostProcessor<,>));

        // ===========================================
        // Registrasi Handler (Command/Query/Notification)
        // ===========================================

        // Handler perlu didaftarkan manual jika tidak auto-scan (umumnya auto lewat AddAIDispatcherCore)
        // builder.Services.AddScoped<IRequestHandler<MyCommand>, MyCommandHandler>();
        // builder.Services.AddScoped<IRequestHandler<MyQuery, MyResult>, MyQueryHandler>();
        // builder.Services.AddScoped<INotificationHandler<MyNotification>, MyNotificationHandler>();

        // ===========================================
        // End Registrasi
        // ===========================================


        var app = builder.Build();
        var dispatcher = app.Services.GetRequiredService<IDispatcher>();

        // ==== UJI COBA SEMUA FITUR DISPATCHER ====
        await TestExceptionBehavior(dispatcher);
        await TestLoggingBehavior(dispatcher);
        await TestTimeoutBehavior(dispatcher);
        await TestPerformanceBehavior(dispatcher);
        await TestRetryBehavior(dispatcher);
        await TestCircuitBreakerBehavior(dispatcher);

        await TestNotificationTimeoutBehavior(dispatcher);
        await TestNotificationExceptionBehavior(dispatcher);
        await TestNotificationPerformanceBehavior(dispatcher);
        await TestNotificationRetryBehavior(dispatcher);
        await TestNotificationCircuitBreakerBehavior(dispatcher);
        await TestNotificationLoggingBehavior(dispatcher);

        // ==== CONTOH ATTRIBUTE PRIORITY & TIMEOUT ====
        await TestNotificationPriority(dispatcher);
        await TestWithTimeoutAttribute(dispatcher);
    }

    // ===================== REQUEST/COMMAND TESTS =====================
    static async Task TestExceptionBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: ExceptionBehavior pada Command ===");
        try
        {
            await dispatcher.Send(new ExceptionCommand());
            Console.WriteLine("[FAILED] Tidak terjadi exception!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PASSED] Exception tertangkap: {ex.GetType().Name}: {ex.Message}\n");
        }
    }
    public record ExceptionCommand() : IRequest;
    public class ExceptionCommandHandler : IRequestHandler<ExceptionCommand>
    {
        public Task Handle(ExceptionCommand request, CancellationToken ct)
            => throw new InvalidOperationException("Simulasi error ExceptionCommand!");
    }

    static async Task TestLoggingBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: LoggingBehavior pada Command ===");
        await dispatcher.Send(new LoggingCommand());
        Console.WriteLine("[PASSED] Cek log console untuk info start/end.\n");
    }
    public record LoggingCommand() : IRequest;
    public class LoggingCommandHandler : IRequestHandler<LoggingCommand>
    {
        public Task Handle(LoggingCommand request, CancellationToken ct)
        {
            Console.WriteLine("[LoggingCommandHandler] Proses command logging.");
            return Task.CompletedTask;
        }
    }

    static async Task TestTimeoutBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: TimeoutBehavior pada Command ===");
        try
        {
            await dispatcher.Send(new TimeoutCommand());
            Console.WriteLine("[FAILED] Tidak terjadi timeout!");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"[PASSED] TimeoutException tertangkap: {ex.Message}\n");
        }
    }
    public record TimeoutCommand() : IRequest;
    public class TimeoutCommandHandler : IRequestHandler<TimeoutCommand>
    {
        public async Task Handle(TimeoutCommand request, CancellationToken ct)
        {
            Console.WriteLine("[TimeoutCommandHandler] Simulasi delay 1200ms...");
            await Task.Delay(1200, ct); // Default timeout biasanya 1s
        }
    }

    static async Task TestPerformanceBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: PerformanceBehavior pada Command ===");
        await dispatcher.Send(new PerformanceCommand());
        Console.WriteLine("[PASSED] Cek log warning jika eksekusi > threshold.\n");
    }
    public record PerformanceCommand() : IRequest;
    public class PerformanceCommandHandler : IRequestHandler<PerformanceCommand>
    {
        public async Task Handle(PerformanceCommand request, CancellationToken ct)
        {
            Console.WriteLine("[PerformanceCommandHandler] Simulasi delay 700ms...");
            await Task.Delay(700, ct);
        }
    }

    static async Task TestRetryBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: RetryBehavior pada Command ===");
        try
        {
            await dispatcher.Send(new RetryCommand(2)); // Gagal 2x, retry 3x (default)
            Console.WriteLine("[FAILED] Tidak terjadi error setelah retry!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PASSED] Handler tetap error setelah retry: {ex.GetType().Name}: {ex.Message}\n");
        }
    }
    public record RetryCommand(int FailCount) : IRequest;
    public class RetryCommandHandler : IRequestHandler<RetryCommand>
    {
        private static int _failCount = 0;
        public Task Handle(RetryCommand request, CancellationToken ct)
        {
            if (_failCount < request.FailCount)
            {
                _failCount++;
                throw new InvalidOperationException($"Simulasi error ke-{_failCount} pada RetryCommand");
            }
            _failCount = 0;
            Console.WriteLine("[RetryCommandHandler] Sukses setelah beberapa kali gagal.");
            return Task.CompletedTask;
        }
    }

    static async Task TestCircuitBreakerBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: CircuitBreakerBehavior pada Command ===");
        for (int i = 1; i <= 5; i++)
        {
            try
            {
                await dispatcher.Send(new CircuitBreakerCommand());
                Console.WriteLine($"[ITERASI {i}] [FAILED] Tidak terjadi error (harusnya error).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ITERASI {i}] Handler error: {ex.GetType().Name}: {ex.Message}");
            }
        }
        Console.WriteLine("[INFO] Cek log circuit breaker.\n");
    }
    public record CircuitBreakerCommand() : IRequest;
    public class CircuitBreakerCommandHandler : IRequestHandler<CircuitBreakerCommand>
    {
        public Task Handle(CircuitBreakerCommand request, CancellationToken ct)
            => throw new InvalidOperationException("Simulasi always fail pada CircuitBreakerCommand.");
    }

    // ===================== NOTIFICATION TESTS =====================
    static async Task TestNotificationTimeoutBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: TimeoutBehavior pada Notification ===");
        try
        {
            await dispatcher.Publish(new TimeoutNotification(2000, "Delay 2 detik, timeout 3 detik")); // Sukses
            Console.WriteLine("[PASSED] Notifikasi selesai sebelum timeout.\n");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"[FAILED] Tidak seharusnya timeout: {ex.Message}\n");
        }
        try
        {
            await dispatcher.Publish(new TimeoutNotification(2000, "Delay 2 detik, timeout default 1.5 detik"));
            Console.WriteLine("[FAILED] Harusnya timeout, tapi sukses.");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"[PASSED] Notifikasi gagal tepat waktu (timeout): {ex.Message}\n");
        }
    }
    public class TimeoutNotification : INotification, ITimeoutAware
    {
        public int DelayMs { get; }
        public string Message { get; }
        public TimeSpan Timeout => _timeout;
        private readonly TimeSpan _timeout;
        public TimeoutNotification(int delayMs, string message)
        {
            DelayMs = delayMs;
            Message = message;
            _timeout = delayMs == 2000 && message.Contains("default") ? TimeSpan.FromMilliseconds(1500) : TimeSpan.FromSeconds(3);
        }
    }
    public class TimeoutNotificationHandler : INotificationHandler<TimeoutNotification>
    {
        public async Task Handle(TimeoutNotification notification, CancellationToken ct)
        {
            Console.WriteLine($"[TimeoutNotificationHandler] {notification.Message}");
            await Task.Delay(notification.DelayMs, ct);
        }
    }

    static async Task TestNotificationExceptionBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: ExceptionBehavior pada Notification ===");
        try
        {
            await dispatcher.Publish(new ExceptionNotification());
            Console.WriteLine("[FAILED] Tidak terjadi exception!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PASSED] Exception tertangkap: {ex.GetType().Name}: {ex.Message}\n");
        }
    }
    public class ExceptionNotification : INotification { }
    public class ExceptionNotificationHandler : INotificationHandler<ExceptionNotification>
    {
        public Task Handle(ExceptionNotification notification, CancellationToken ct)
            => throw new InvalidOperationException("Simulasi error pada ExceptionNotification!");
    }

    static async Task TestNotificationPerformanceBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: PerformanceBehavior pada Notification ===");
        await dispatcher.Publish(new PerformanceNotification(800));
        Console.WriteLine("[INFO] Lihat log warning jika melebihi ambang performance.\n");
    }
    public class PerformanceNotification : INotification
    {
        public int DelayMs { get; }
        public PerformanceNotification(int delayMs) => DelayMs = delayMs;
    }
    public class PerformanceNotificationHandler : INotificationHandler<PerformanceNotification>
    {
        public async Task Handle(PerformanceNotification notification, CancellationToken ct)
        {
            Console.WriteLine($"[PerformanceNotificationHandler] Simulasi delay {notification.DelayMs} ms");
            await Task.Delay(notification.DelayMs, ct);
        }
    }

    static async Task TestNotificationRetryBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: RetryBehavior pada Notification ===");
        try
        {
            await dispatcher.Publish(new RetryNotification(3)); // Gagal 3x
            Console.WriteLine("[FAILED] Tidak terjadi error setelah retry!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PASSED] Handler tetap error setelah retry: {ex.GetType().Name}: {ex.Message}\n");
        }
    }
    public class RetryNotification : INotification
    {
        public int ErrorTimes { get; }
        public RetryNotification(int errorTimes) => ErrorTimes = errorTimes;
    }
    public class RetryNotificationHandler : INotificationHandler<RetryNotification>
    {
        private static int _failCount = 0;
        public Task Handle(RetryNotification notification, CancellationToken ct)
        {
            if (_failCount < notification.ErrorTimes)
            {
                _failCount++;
                throw new InvalidOperationException($"Simulasi error ke-{_failCount} pada RetryNotification");
            }
            _failCount = 0;
            Console.WriteLine("[RetryNotificationHandler] Sukses setelah beberapa kali gagal.");
            return Task.CompletedTask;
        }
    }

    static async Task TestNotificationCircuitBreakerBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: CircuitBreakerBehavior pada Notification ===");
        for (int i = 1; i <= 4; i++)
        {
            try
            {
                await dispatcher.Publish(new CircuitBreakerNotification());
                Console.WriteLine($"[ITERASI {i}] [FAILED] Tidak terjadi error!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ITERASI {i}] Handler error: {ex.GetType().Name}: {ex.Message}");
            }
        }
        Console.WriteLine("[INFO] Cek log circuit breaker notification.\n");
    }
    public class CircuitBreakerNotification : INotification { }
    public class CircuitBreakerNotificationHandler : INotificationHandler<CircuitBreakerNotification>
    {
        public Task Handle(CircuitBreakerNotification notification, CancellationToken ct)
            => throw new InvalidOperationException("Simulasi always fail pada CircuitBreakerNotification!");
    }

    static async Task TestNotificationLoggingBehavior(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: LoggingBehavior pada Notification ===");
        await dispatcher.Publish(new LoggingNotification("Contoh pesan log pada notification."));
        Console.WriteLine("[PASSED] Cek log notifikasi di output.\n");
    }
    public class LoggingNotification : INotification
    {
        public string Message { get; }
        public LoggingNotification(string message) => Message = message;
    }
    public class LoggingNotificationHandler : INotificationHandler<LoggingNotification>
    {
        public Task Handle(LoggingNotification notification, CancellationToken ct)
        {
            Console.WriteLine($"[LoggingNotificationHandler] {notification.Message}");
            return Task.CompletedTask;
        }
    }





    // ===================== ATTRIBUTE TESTS =====================
    static async Task TestNotificationPriority(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: WithPriorityAttribute pada Notification ===");
        await dispatcher.Publish(new DemoPriorityNotification());
        Console.WriteLine("[INFO] Handler dijalankan urut prioritas (cek urutan output di bawah).\n");
    }
    public class DemoPriorityNotification : INotification { }

    [WithPriority(-10)]
    public class FirstHandler : INotificationHandler<DemoPriorityNotification>
    {
        public Task Handle(DemoPriorityNotification notification, CancellationToken ct)
        {
            Console.WriteLine("[FirstHandler] PRIORITY -10 (harus lebih awal)");
            return Task.CompletedTask;
        }
    }

    [WithPriority(20)]
    public class SecondHandler : INotificationHandler<DemoPriorityNotification>
    {
        public Task Handle(DemoPriorityNotification notification, CancellationToken ct)
        {
            Console.WriteLine("[SecondHandler] PRIORITY 20 (harus lebih akhir)");
            return Task.CompletedTask;
        }
    }

    static async Task TestWithTimeoutAttribute(IDispatcher dispatcher)
    {
        Console.WriteLine("\n=== TEST: WithTimeoutAttribute pada Notification ===");
        try
        {
            await dispatcher.Publish(new AttributeTimeoutNotification());
            Console.WriteLine("[FAILED] Tidak terjadi timeout! (Handler terlalu cepat atau pipeline belum aktif)");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"[PASSED] TimeoutException tertangkap (karena attribute): {ex.Message}\n");
        }
    }

    [WithTimeout(500)] // Timeout 500 ms (lebih pendek dari delay handler)
    public class AttributeTimeoutNotification : INotification { }

    public class AttributeTimeoutNotificationHandler : INotificationHandler<AttributeTimeoutNotification>
    {
        public async Task Handle(AttributeTimeoutNotification notification, CancellationToken ct)
        {
            Console.WriteLine("[AttributeTimeoutNotificationHandler] Simulasi delay 1000ms (harus timeout)");
            await Task.Delay(1000, ct);
        }
    }
}
