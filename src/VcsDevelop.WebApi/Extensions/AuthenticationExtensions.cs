using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using VcsDevelop.Infrastructure.Auth;
using VcsDevelop.WebApi.Options;

namespace VcsDevelop.WebApi.Extensions;

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
