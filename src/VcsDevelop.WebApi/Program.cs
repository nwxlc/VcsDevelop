using Serilog;

using VcsDevelop.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder
    .AddSerilog()
    .AddServices();

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.ConfigureMiddleware();
app.ConfigureSpa();

app.Run();