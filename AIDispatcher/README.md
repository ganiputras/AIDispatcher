![AIDispatcher Logo](https://raw.githubusercontent.com/ganiputras/AIDispatcher/master/logo.png)

# AIDispatcher

**Modern, modular, and extensible CQRS Dispatcher for .NET 8+**

[![NuGet](https://img.shields.io/nuget/v/AIDispatcher.svg?style=flat-square)](https://www.nuget.org/packages/AIDispatcher)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://github.com/ganiputras/AIDispatcher/blob/master/AIDispatcher/LICENSE.txt)


## 🚀 AIDispatcher: CQRS & Notification Pipeline for .NET 8+

AIDispatcher adalah library .NET open-source untuk CQRS, Pipeline Behavior, dan Notification Dispatcher modern — terinspirasi MediatR, namun lebih modular, lebih fleksibel, dan lebih mudah dikembangkan.

## ✨ Fitur Utama

- Pipeline Behavior Modular: Logging, Retry, Timeout, Circuit Breaker, Exception Handling, Performance Monitoring, Pre/Post Processor
- Notification Dispatcher: Parallel & Sequential, Prioritas Handler (`WithPriority`), pipeline untuk logging, timeout, retry, dsb
- Request Dispatcher (CQRS): Handler dengan Response & Void, pipeline modular
- Extensible & Plug & Play: Mudah menambah pipeline/behavior via DI
- Dokumentasi XML lengkap (Bahasa Indonesia): semua public API terdokumentasi untuk IntelliSense

## 📦 Instalasi

Install via NuGet:

```sh
dotnet add package AIDispatcher
```

 ## 🛠️ Contoh Penggunaan
Request Handler (CQRS)
```sh
public class GetOrderQuery : IRequest<OrderDto>
{
    public int Id { get; set; }
}

public class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderDto>
{
    public Task<OrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        // Query ke database atau source data lain
        return Task.FromResult(new OrderDto { ... });
    }
}

// Kirim request
var order = await dispatcher.Send<GetOrderQuery, OrderDto>(new GetOrderQuery { Id = 1 });    
```




## ⚡ Pipeline Built-in

- Logging: Catat request, notifikasi, dan durasi eksekusi

- Retry: Otomatis ulangi jika gagal (Polly)

- Timeout: Batasi waktu maksimal eksekusi

- Circuit Breaker: Putus eksekusi jika error berturut-turut

- Performance: Warning jika eksekusi lambat

- Pre/Post Processor: Hook sebelum/sesudah handler berjalan

- Exception Handling: Tangani error secara global

- Notification Priority: Eksekusi handler sesuai prioritas

##  🆚 Fitur

| Fitur                        | AIDispatcher |
| ---------------------------- | ------------ |
| Request/Response             | ✔️           |
| Notification/Publish         | ✔️           |
| Pipeline Modular             | ✔️           |
| Logging/Performance Pipeline | ✔️           |
| Retry/Circuit Breaker/Polly  | ✔️           |
| Exception Handling Pipeline  | ✔️           |
| Notification Priority        | ✔️           |
| Notification Parallel/Seq    | ✔️           |
| XML Doc Bahasa Indonesia     | ✔️           |
| Extensible Pipeline          | ✔️           |
| DI Friendly                  | ✔️           |
| Blazor Friendly              | ✔️           |


AIDispatcher menghadirkan keunggulan dengan penambahan fitur advanced seperti parallel notification, priority, built-in retry/circuit breaker, dan pipeline monitoring, siap produksi di aplikasi Anda.


##  💡 Migrasi dari MediatR
- Interface mirip (IRequest, INotification, IRequestHandler, INotificationHandler)

- Pipeline behavior mirip MediatR, lebih mudah di-extend

- Tidak perlu konfigurasi rumit, tinggal ganti DI dan handler

##    📚 Lisensi
MIT © 2025 Gani Putras
Kontribusi & feedback sangat diterima!
