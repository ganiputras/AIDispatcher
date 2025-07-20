using AIDispatcher.Dispatcher;
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
//builder.Services.AddAIDispatcher(typeof(CreateUserValidator).Assembly); 
//builder.Services.AddAIDispatcher(typeof(Program).Assembly);
//builder.Services.AddAIDispatcher(options =>
//{
//    options.ParallelNotificationHandlers = true;
//    options.NotificationHandlerPriorityEnabled = true;
//});

builder.Services.AddAIDispatcher(options =>
{
    options.ParallelNotificationHandlers = true;
    options.NotificationHandlerPriorityEnabled = true;
}, typeof(Program).Assembly);

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

var result = await dispatcher.SendAsync<CreateUserCommand, string>(
    new CreateUserCommand { Name = "Gani", Email = "gani@domain.com" });

Console.WriteLine($"Response: {result}");

await app.RunAsync();