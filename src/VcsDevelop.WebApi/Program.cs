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

await app.EnsureMinioBucketExistsAsync();
await app.ApplyMigrationsAsync();

app.ConfigureMiddleware();

app.UseDefaultFiles(); 
app.UseStaticFiles(); 

app.UseRouting();

app.MapFallbackToFile("index.html");

app.Run();
