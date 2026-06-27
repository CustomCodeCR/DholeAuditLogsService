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

const string CorsPolicyName = "DholeWebCors";

builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

builder.Services.AddCustomCodeApiWithSwagger(title: "Dhole Audit Logs Service", version: "v1");

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        CorsPolicyName,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});


builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseCustomCodeApi();

app.UseCors(CorsPolicyName);

if (app.Environment.IsDevelopment())
{
    app.UseCustomCodeSwagger();
}

app.MapGet(
        "/health",
        () =>
        {
            return Results.Ok(
                new
                {
                    service = "DholeAuditLogsService",
                    status = "Healthy",
                    timestamp = DateTimeOffset.UtcNow,
                }
            );
        }
    )
    .AllowAnonymous();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuditEventEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();

    await dbContext.Database.MigrateAsync();
}

app.Run();
