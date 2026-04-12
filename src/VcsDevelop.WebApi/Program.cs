using Serilog;
using VcsDevelop.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services
    .AddServices(builder.Configuration);

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.ConfigureMiddleware();

app.Run();
