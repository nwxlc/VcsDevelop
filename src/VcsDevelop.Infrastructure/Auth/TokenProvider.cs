using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using VcsDevelop.Application.Accounts.Auth;
using VcsDevelop.Domain.Accounts;
using VcsDevelop.Infrastructure.Options.Tokens;

namespace VcsDevelop.Infrastructure.Auth;

public sealed class TokenProvider : ITokenProvider
{
    private readonly ITokenSettings _tokenSettings;

    public TokenProvider(ITokenSettings tokenSettings)
    {
        ArgumentNullException.ThrowIfNull(tokenSettings);

        _tokenSettings = tokenSettings;
    }

    public Token CreateToken(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);

        var jwtDate = DateTime.UtcNow;
        var expiresAt = jwtDate.Add(_tokenSettings.AccessTokenLifetime);
        var claims = BuildClaims(account).ToArray();

        var signingCredentials = CreateSigningCredentials();

        var jwt = new JwtSecurityToken(
            _tokenSettings.Issuer,
            _tokenSettings.Audience,
            claims,
            jwtDate,
            expiresAt,
            signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return Token.Create(token, expiresAt);
    }

    private SigningCredentials CreateSigningCredentials()
    {
        using var rsa = RSA.Create();
        var privateKey = Convert.FromBase64String(_tokenSettings.PrivateKey);
        rsa.ImportPkcs8PrivateKey(privateKey, out _);

        var securityKey = new RsaSecurityKey(rsa.ExportParameters(includePrivateParameters: true));
        return new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
    }

    private IEnumerable<Claim> BuildClaims(Account account)
    {
        yield return new Claim(_tokenSettings.AccountIdClaimName, account.Id.ToString());

        yield return new Claim(JwtRegisteredClaimNames.Email, account.Email);
        
        yield return new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString());
    }
}
