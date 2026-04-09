using VcsDevelop.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddServices(builder.Configuration);

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.ConfigureMiddleware();

app.Run();
