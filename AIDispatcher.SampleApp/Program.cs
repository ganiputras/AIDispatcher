

using AIDispatcher;
using AIDispatcher.SampleApp.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

// Logging
builder.Logging.AddConsole();

// Registrasi 
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


// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("AIDispatcher.SampleApp"))
    .WithTracing(t =>
    {
        t.AddSource("AIDispatcher");
        t.AddHttpClientInstrumentation();
        t.AddAspNetCoreInstrumentation();
        t.AddConsoleExporter();
    })
    .WithMetrics(m =>
    {
        m.AddMeter("AIDispatcher");
        m.AddHttpClientInstrumentation();
        m.AddAspNetCoreInstrumentation();
    });

var app = builder.Build();

// Contoh penggunaan dispatcher
using var scope = app.Services.CreateScope();
var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();

var result1 = await dispatcher.SendAsync<CreateUserCommand, string>(new CreateUserCommand
{ Name = "Gani", Email = "gani@domain.com" });

var result3 = await dispatcher.SendAsync<CreateUserMediatrStyleCommand, string>(new CreateUserMediatrStyleCommand
{ Name = "Gani Style", Email = "gani@domain.com" });

Console.WriteLine($"Response: {result1}");
Console.WriteLine($"Response: {result3}");


await app.RunAsync();