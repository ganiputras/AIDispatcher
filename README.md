# AIDispatcher

AIDispatcher adalah framework ringan berbasis .NET 8 yang menggantikan MediatR untuk CQRS, pipeline behavior, dan event notification. Dirancang dengan performa tinggi, dukungan parallel, prioritas handler, dan pengembangan modular.

## 🚀 Fitur Utama

* **Command / Query Dispatcher** (dengan dukungan pipeline)
* **Event Notification Dispatcher** (dengan dukungan paralel & prioritas)
* **Built-in Behaviors**:

  * Validation
  * Tracing
  * Retry
  * Timeout
  * Circuit Breaker
  * Metrics
  * Logging (via NotificationBehavior)
* **Dukungan Pipeline Modular** untuk extensibility
* **Support Dependency Injection** (termasuk `IDispatcherRoot` scope-less)
* **Performance-friendly**: Tanpa refleksi berlebihan

---

## 📦 Instalasi

```
Install-Package AIDispatcher
```

---

## 🧩 Konfigurasi Dasar

Di dalam `Program.cs`:

```csharp
builder.Services.AddAIDispatcher(typeof(Program).Assembly);
```

Atau dengan opsi lanjutan:

```csharp
builder.Services.AddAIDispatcher(options =>
{
    options.ParallelNotificationHandlers = true;
    options.NotificationHandlerPriorityEnabled = true;
}, typeof(Program).Assembly);
```

---

## 🧪 Contoh Penggunaan

### Command / Query

```csharp
public record CreateUser(string Name) : IDispatcherRequest<Guid>;

public class CreateUserHandler : IDispatcherHandler<CreateUser, Guid>
{
    public Task<Guid> Handle(CreateUser request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Guid.NewGuid());
    }
}
```

Panggil dengan:

```csharp
var result = await dispatcher.SendAsync<CreateUser, Guid>(new CreateUser("Test User"));
```

---

### Notification / Event

```csharp
public record UserCreated(Guid Id) : INotification;

public class SendWelcomeEmailHandler : INotificationHandler<UserCreated>
{
    public Task Handle(UserCreated notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Welcome email sent to user: {notification.Id}");
        return Task.CompletedTask;
    }
}
```

Panggil:

```csharp
await dispatcher.PublishAsync(new UserCreated(userId));
```

---

## ⚙️ Notification Pipeline

### Contoh Logging Behavior

```csharp
public class LoggingNotificationBehavior<T> : INotificationBehavior<T> where T : notnull
{
    public async Task HandleAsync(T notification, CancellationToken ct, Func<Task> next)
    {
        Console.WriteLine($"[LOG] Start: {typeof(T).Name}");
        await next();
        Console.WriteLine($"[LOG] End: {typeof(T).Name}");
    }
}
```

### Retry Notification Behavior

```csharp
public class RetryNotificationBehavior<T> : INotificationBehavior<T> where T : notnull
{
    public async Task HandleAsync(T notification, CancellationToken ct, Func<Task> next)
    {
        int retries = 3;
        while (true)
        {
            try
            {
                await next();
                return;
            }
            catch when (--retries > 0)
            {
                Console.WriteLine($"Retrying {typeof(T).Name}...");
            }
        }
    }
}
```

---

## 🧠 Advanced

* `IDispatcherRoot`: digunakan di background service / hosted / entry-point:

```csharp
var result = await app.Services.GetRequiredService<IDispatcherRoot>().SendAsync<SomeCommand, string>(...);
```

* `NotificationHandlerPriorityEnabled`: jalankan handler berdasarkan prioritas jika diaktifkan
* `ParallelNotificationHandlers`: jalankan handler notifikasi secara paralel

---

## 📁 Struktur Interface Utama

```csharp
// Command / Query
IDispatcherRequest<TResult>
IDispatcherHandler<TRequest, TResult>

// Notification
INotification
INotificationHandler<TNotification>
INotificationBehavior<TNotification>
INotificationHandlerWithPriority (optional)
```

---

## 🏁 Roadmap

* [x] Dispatcher Core
* [x] Notification Parallel
* [x] Notification Priority
* [x] Notification Pipeline
* [ ] Unit Test + Sample
* [ ] Source Generator (planned)
* [ ] Benchmark Performance vs MediatR

---

## 🧾 Lisensi

MIT License
