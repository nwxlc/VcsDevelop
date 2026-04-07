using System.Security.Cryptography;
using System.Text;
using GitDevelop.Domain.Accounts;

namespace GitDevelop.Infrastructure.Auth;

public sealed class RefreshToken
{
    public string TokenHash { get; private init; }
    public Guid UserId { get; private init; }
    public DateTimeOffset ExpiresAt { get; private init; }

    private RefreshToken(
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }
    
    public static RefreshToken Create(
        Token token,
        Guid userId)
    {
        ArgumentNullException.ThrowIfNull(token);

        var tokenHash = ComputeHash(token.Value);

        return new RefreshToken(
            userId,
            tokenHash,
            token.ExpirationDate
            ?? throw new InvalidOperationException("Token expiration date is required"));
    }

    public static string ComputeHash(string token)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
