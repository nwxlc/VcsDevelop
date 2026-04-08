using GitDevelop.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddServices(builder.Configuration);

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.ConfigureMiddleware();

app.UseDefaultFiles(); 
app.UseStaticFiles(); 

app.UseRouting();

app.MapFallbackToFile("index.html");

app.Run();