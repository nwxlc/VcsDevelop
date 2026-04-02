using GitDevelop.Core.Application;
using GitDevelop.Infrastructure.Options.Tokens;

namespace GitDevelop.WebApi.Contexts;

public class RequestContext : IRequestContext
{
    public Guid? AccountId => GetAccountId();

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenSettings _tokenSettings;

    public RequestContext(
        IHttpContextAccessor httpContextAccessor,
        ITokenSettings tokenSettings)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        ArgumentNullException.ThrowIfNull(tokenSettings);

        _httpContextAccessor = httpContextAccessor;
        _tokenSettings = tokenSettings;
    }

    public Guid GetRequiredAccountId()
    {
        return AccountId ?? throw new UnauthorizedAccessException("Account ID is required but was not found.");
    }

    private Guid? GetAccountId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(_tokenSettings.AccountIdClaimName);
        return userIdClaim is not null
            ? Guid.Parse(userIdClaim.Value)
            : null;
    }
}
