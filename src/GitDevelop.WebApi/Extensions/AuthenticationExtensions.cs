using GitDevelop.Infrastructure.Auth;
using GitDevelop.WebApi.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace GitDevelop.WebApi.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthentication(
        this IServiceCollection service,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(service);

        service
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

        service.AddSingleton<ITokenValidationParametersFactory, TokenValidationParametersFactory>();
        service.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        return service;
    }
}
