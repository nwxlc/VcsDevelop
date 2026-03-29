using GitDevelop.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddServices(builder.Configuration);

var app = builder.Build();

app.ConfigureMiddleware();

app.Run();
