using AIDispatcher;
using AIDispatcher.SampleApp.Handlers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

// Logging
builder.Logging.AddConsole();


var distinctAssemblies = AppDomain.CurrentDomain
    .GetAssemblies()
    .Where(a => !a.IsDynamic)
    .GroupBy(a => a.FullName)
    .Select(g => g.First());

foreach (var assembly in distinctAssemblies)
{
    builder.Services.AddValidatorsFromAssembly(assembly);
}


//builder.Services.AddValidatorsFromAssembly(typeof(CreateUserCommand).Assembly);
// Registrasi 
builder.Services.AddAIDispatcher(assemblies: typeof(Program).Assembly);

//builder.Services.AddAIDispatcher(
//    builder =>
//    {
//        builder.UseValidation();
//        builder.UseRetry(opts => opts.MaxRetries = 3);
//        builder.UseTimeout(TimeSpan.FromSeconds(2));
//    },
//    options =>
//    {
//        options.ParallelNotificationHandlers = true;
//        options.NotificationHandlerPriorityEnabled = true;
//    },
//    typeof(Program).Assembly
//);

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
{ Name = "", Email = "gani@domain.com", Phone = "010100101", Address = "Jawa Tengah" });

var result3 = await dispatcher.SendAsync<CreateUserMediatrStyleCommand, string>(new CreateUserMediatrStyleCommand
{ Name = "Gani Style", Email = "gani@domain.com", Phone = "010100101", Address = "Jawa Tengah" });

Console.WriteLine($"Response: {result1}");
Console.WriteLine($"Response: {result3}");


await app.RunAsync();