using CustomCodeFramework.Api.DependencyInjection;
using CustomCodeFramework.Api.Swagger;
using CustomCodeFramework.Core.Abstractions;
using Dhole.AuditLogs.Api.Endpoints;
using Dhole.AuditLogs.Application.DependencyInjection;
using Dhole.AuditLogs.Infrastructure.DependencyInjection;
using Dhole.AuditLogs.Infrastructure.Time;
using Dhole.AuditLogs.Persistence.DbContexts;
using Dhole.AuditLogs.Persistence.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

builder.Services.AddCustomCodeApiWithSwagger(title: "Dhole Audit Logs Service", version: "v1");

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseCustomCodeApi();

if (app.Environment.IsDevelopment())
{
    app.UseCustomCodeSwagger();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapAuditEventEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();

    await dbContext.Database.MigrateAsync();
}

app.Run();
