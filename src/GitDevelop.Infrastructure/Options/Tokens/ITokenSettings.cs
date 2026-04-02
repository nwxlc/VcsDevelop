namespace GitDevelop.Infrastructure.Options.Tokens;

public interface ITokenSettings
{
    string Issuer { get; }
    string Audience { get; }
    string PrivateKey { get; }
    string PublicKey { get; }
    TimeSpan AccessTokenLifetime { get; }
    string AccountIdClaimName { get; }
}
