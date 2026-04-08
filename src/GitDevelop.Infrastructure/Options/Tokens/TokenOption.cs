namespace GitDevelop.Infrastructure.Options.Tokens;

public class TokenOption : ITokenSettings
{
    public const string Name = "JwtToken";
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string PrivateKey { get; init; } = string.Empty;
    public string PublicKey { get; init; } = string.Empty;
    public TimeSpan AccessTokenLifetime { get; init; } = TimeSpan.Zero;
    public TimeSpan RefreshTokenLifetime { get; init; } = TimeSpan.Zero;
    public string AccountIdClaimName => "account_id";
}
