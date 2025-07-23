# AIDispatcher

[![NuGet](https://img.shields.io/nuget/v/AIDispatcher?color=green&logo=nuget)](https://www.nuget.org/packages/AIDispatcher)
[![Build Status](https://github.com/ganiputras/AIDispatcher/workflows/Build/badge.svg)](https://github.com/ganiputras/AIDispatcher/actions)
![.NET 8+](https://img.shields.io/badge/.NET-8.0%2B-blueviolet)
![MIT License](https://img.shields.io/badge/License-MIT-lightgray.svg)

> **AIDispatcher** adalah framework **CQRS & pipeline** open source untuk .NET 8+  
> Lebih simpel, fleksibel, dan powerful.  
> Plug & play — Registrasi cukup 1 baris, pipeline modular, handler auto-discover, siap untuk microservice, Blazor, Console, WebAPI.
> 
> Framework dispatcher open source berbasis .NET 8+ yang mengusung arsitektur CQRS dan pipeline modular.
> Terinspirasi dari MediatR — menawarkan pengalaman yang lebih simpel, terotomatisasi, serta pipeline yang mudah dikembangkan.
> Semua handler & pipeline didaftarkan otomatis, logging & monitoring siap produksi, serta support fitur advanced seperti timeout, retry, dan circuit breaker (Polly ready).
> Cukup satu baris untuk mulai — langsung siap digunakan di microservice, Blazor, Console, WebAPI, maupun aplikasi enterprise.

## Requirements
- .NET 8.0 or newer
- 
## 🚀 Fitur Utama

- **Plug & Play**: Satu baris registrasi, langsung jalan
- **Pipeline Modular**: Exception, Logging, Timeout, Performance, Retry, Circuit Breaker, dsb
- **Auto-Discovery**: Handler & pipeline otomatis terdaftar
- **Priority & Parallel Notification**
- **Timeout & Cancellation per-request/per-notification**
- **ILogger Integration**: Log siap production
- **Polly Ready**: Retry & circuit breaker support
- **Mudah di-extend** (pipeline & handler custom)
- **Siap untuk microservice, Blazor, Console, WebAPI, dll**


## 📦 Instalasi

1. **Tambahkan NuGet Package**
    ```bash
    dotnet add package AIDispatcher
    dotnet add package Polly
    ```

2. **Registrasi di DI (Dependency Injection)**
    ```csharp
    // Pipeline utama (wajib)
    builder.Services.AddAIDispatcherCore();

    // Pipeline advanced/optional (performance, retry, circuit breaker, dsb)
    builder.Services.AddAIDispatcherAdvanced();
    ```



## ⚡️ Quick Start

**Command & Handler**
```csharp
public record PingCommand(string Message) : IRequest<string>;
public class PingCommandHandler : IRequestHandler<PingCommand, string>
{
    public Task<string> Handle(PingCommand request, CancellationToken ct)
        => Task.FromResult($"Reply: {request.Message}");
}
 ```

**Notification & Handler**
```csharp
public class NotifyMe : INotification
{
    public string Message { get; }
    public NotifyMe(string message) => Message = message;
}
public class NotifyMeHandler : INotificationHandler<NotifyMe>
{
    public Task Handle(NotifyMe notification, CancellationToken ct)
    {
        Console.WriteLine($"[NotifyMeHandler] {notification.Message}");
        return Task.CompletedTask;
    }
}
 ```

**Memanggil di Program.cs**
```csharp
var dispatcher = app.Services.GetRequiredService<IDispatcher>();
var result = await dispatcher.Send<PingCommand, string>(new PingCommand("Hello!"));
await dispatcher.Publish(new NotifyMe("Halo dari AIDispatcher!"));
 ```


## 🏗 Pipeline Bawaan

| Pipeline                          | Command | Notification                            | Fungsionalitas            |
| --------------------------------- | ------- | --------------------------------------- | ------------------------- |
| ExceptionBehavior                 | ✔️      | ✔️ (NotificationExceptionBehavior)      | Penanganan global error   |
| LoggingBehavior                   | ✔️      | ✔️ (LoggingNotificationBehavior)        | Logging start/end         |
| TimeoutBehavior                   | ✔️      | ✔️ (NotificationTimeoutBehavior)        | Timeout per-request       |
| PerformanceBehavior (optional)    | ✔️      | ✔️ (NotificationPerformanceBehavior)    | Warning slow handler      |
| RetryBehavior (optional)          | ✔️      | ✔️ (RetryNotificationBehavior)          | Otomatis retry saat error |
| CircuitBreakerBehavior (optional) | ✔️      | ✔️ (CircuitBreakerNotificationBehavior) | Proteksi overload         |


## 🚦 Urutan Registrasi Pipeline
Pipeline dijalankan sesuai urutan pendaftaran di DI.
Pipeline yang ingin jadi paling luar (misal Exception/Logging) daftarkan TERAKHIR.
```csharp
services.TryAddTransient(typeof(INotificationPipelineBehavior<>), typeof(NotificationTimeoutBehavior<>));
services.TryAddTransient(typeof(INotificationPipelineBehavior<>), typeof(NotificationExceptionBehavior<>)); // outermost
 ```



## 🧪 Contoh Test Pipeline
```csharp
var builder = Host.CreateApplicationBuilder();
builder.Services.AddAIDispatcherCore();
builder.Services.AddAIDispatcherAdvanced();

var app = builder.Build();
var dispatcher = app.Services.GetRequiredService<IDispatcher>();

// Uncomment untuk menjalankan test
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
 ```
```csharp
=== TEST: Timeout pada Notification ===

[SKENARIO 1] Notifikasi timeout 3 detik, handler delay 2 detik (harus selesai).
[Task Handle] Notifikasi timeout 3 detik (simulasi delay 2 detik)
[BERHASIL] Notifikasi selesai sebelum timeout.

[SKENARIO 2] Notifikasi timeout default 1.5 detik, handler delay 2 detik (harus timeout).
warn: AIDispatcher.Core.Behaviors.NotificationTimeoutBehavior[0]
      Notification handler Test_NotificationTimeout timed out after 1500 ms.
[BERHASIL] Notifikasi gagal tepat waktu (timeout): Notification Test_NotificationTimeout exceeded the timeout of 1500 ms.

 ```

## 💡 Kustomisasi & Ekstensi

- Buat pipeline baru: implement IPipelineBehavior<,>, INotificationPipelineBehavior<>, dst.
- Pipeline otomatis terdaftar sesuai urutan di DI.
- Bisa override pipeline, tambah logging, validasi, rate limiting, dsb.


## 📝 FAQ
- Q: Apakah AIDispatcher bisa drop-in replace MediatR?
- A: Ya, tinggal ganti dependency DI dan interface-nya (lihat sample).

- Q: Apakah support paralel handler untuk notification?
- A: Bisa, cek opsi DispatcherOptions.PublishStrategy.

- Q: Cara pakai timeout per-request/per-notification?
- A: Implement interface ITimeoutAware pada request/notification.

- Q: Support .NET 8 / Blazor?
- A: 100% support.


## ✨ Kontribusi
- Dukung project ini via GitHub Sponsor
- Follow @ganiputras di GitHub

## Lisensi
MIT License
