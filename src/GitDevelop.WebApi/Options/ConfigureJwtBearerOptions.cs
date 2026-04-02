using GitDevelop.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace GitDevelop.WebApi.Options;

public sealed class ConfigureJwtBearerOptions
    : IConfigureOptions<JwtBearerOptions>
{
    private readonly ITokenValidationParametersFactory _factory;

    public ConfigureJwtBearerOptions(ITokenValidationParametersFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        
        _factory = factory;
    }

    public void Configure(JwtBearerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.RequireHttpsMetadata = true;
        options.SaveToken = false;
        options.IncludeErrorDetails = true;

        options.TokenValidationParameters = _factory.CreateValidationParameters();
    }
}
