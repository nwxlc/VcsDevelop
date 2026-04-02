using System.Security.Cryptography;
using GitDevelop.Infrastructure.Options.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace GitDevelop.Infrastructure.Auth;

public class TokenValidationParametersFactory : ITokenValidationParametersFactory
{
    private readonly ITokenSettings _settings;

    public TokenValidationParametersFactory(ITokenSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        _settings = settings;
    }

    public TokenValidationParameters CreateValidationParameters()
    {
        using var rsa = RSA.Create();
        var publicKey = Convert.FromBase64String(_settings.PublicKey);
        rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
        var securityKey = new RsaSecurityKey(rsa.ExportParameters(includePrivateParameters: false));

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,

            IssuerSigningKey = securityKey
        };
    }
}
