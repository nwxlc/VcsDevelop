using Microsoft.IdentityModel.Tokens;

namespace GitDevelop.Infrastructure.Auth;

public interface ITokenValidationParametersFactory
{
    TokenValidationParameters CreateValidationParameters();
}
