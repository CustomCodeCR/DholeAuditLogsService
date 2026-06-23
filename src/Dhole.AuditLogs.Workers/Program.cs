using CustomCodeFramework.Core.Abstractions;
using Dhole.AuditLogs.Application.DependencyInjection;
using Dhole.AuditLogs.Infrastructure.Time;
using Dhole.AuditLogs.Persistence.DependencyInjection;
using Dhole.AuditLogs.Worker.DependencyInjection;
using Dhole.AuditLogs.Workers.Security;

var contentRoot = Path.Combine(Directory.GetCurrentDirectory(), "src", "Dhole.AuditLogs.Workers");

if (!Directory.Exists(contentRoot))
{
    contentRoot = Path.Combine(Directory.GetCurrentDirectory(), "src", "Dhole.AuditLogs.Worker");
}

if (!Directory.Exists(contentRoot))
{
    contentRoot = Directory.GetCurrentDirectory();
}

var builder = Host.CreateApplicationBuilder(
    new HostApplicationBuilderSettings { Args = args, ContentRootPath = contentRoot }
);

builder.Configuration.Sources.Clear();

builder
    .Configuration.SetBasePath(contentRoot)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

Console.WriteLine($"Postgres: {builder.Configuration["Postgres:ConnectionString"]}");

builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddScoped<ICurrentUser, WorkerCurrentUser>();

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddAuditLogsWorker(builder.Configuration);

var host = builder.Build();

await host.RunAsync();
