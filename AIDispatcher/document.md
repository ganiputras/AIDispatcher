📌 Contoh Registrasi AIDispatcher

✅ 1. Default Setup (tanpa konfigurasi apa pun)

Jika Anda hanya ingin mendaftarkan handler dan pipeline default:

builder.Services.AddAIDispatcher(assemblies: typeof(Program).Assembly);

✅ 2. Hanya DispatcherOptions

Mengubah perilaku notifikasi (tanpa pipeline custom):

builder.Services.AddAIDispatcher(
    configureOptions: options =>
    {
        options.ParallelNotificationHandlers = false;
        options.NotificationHandlerPriorityEnabled = true;
    },
    assemblies: typeof(Program).Assembly
);

✅ 3. Hanya Fluent Builder (tanpa DispatcherOptions)

Menentukan pipeline secara deklaratif:

builder.Services.AddAIDispatcher(
    builder =>
    {
        builder
            .UseValidation()
            .UseRetry(opts =>
            {
                opts.MaxRetries = 5;
                opts.Delay = TimeSpan.FromMilliseconds(300);
            })
            .UseTimeout(TimeSpan.FromSeconds(2));
    },
    assemblies: typeof(Program).Assembly
);

✅ 4. Registrasi dari Multiple Assemblies

Jika handler tersebar di lebih dari satu project:

builder.Services.AddAIDispatcher(
    builder =>
    {
        builder.UseValidation();
        builder.UseRetry(opts => opts.MaxRetries = 3);
        builder.UseTimeout(TimeSpan.FromSeconds(2));
    },
    options =>
    {
        options.ParallelNotificationHandlers = true;
        options.NotificationHandlerPriorityEnabled = true;
    },
    typeof(Program).Assembly
);

