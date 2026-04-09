using VcsDevelop.Domain.Accounts;

namespace VcsDevelop.Application.Accounts.Auth;

public interface ITokenProvider
{
    Token CreateToken(Account account);
}
