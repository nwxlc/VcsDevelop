namespace VcsDevelop.WebApi.Contracts.Accounts;

public class RefreshAccessTokenRequest
{
    public required string RefreshToken { get; init; }
}
