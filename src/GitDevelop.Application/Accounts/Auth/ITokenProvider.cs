using GitDevelop.Domain.Accounts;

namespace GitDevelop.Application.Accounts.Auth;

public interface ITokenProvider
{
    Token CreateToken(Account account);
}
