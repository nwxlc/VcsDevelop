using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using VcsDevelop.Infrastructure.Auth;

namespace VcsDevelop.WebApi.Options;

public sealed class ConfigureJwtBearerOptions
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly ITokenValidationParametersFactory _factory;

    public ConfigureJwtBearerOptions(ITokenValidationParametersFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.RequireHttpsMetadata = true;
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = _factory.CreateValidationParameters();
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(string.Empty, options);
    }
}
