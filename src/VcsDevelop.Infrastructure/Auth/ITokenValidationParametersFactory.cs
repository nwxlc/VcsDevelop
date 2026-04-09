using Microsoft.IdentityModel.Tokens;

namespace VcsDevelop.Infrastructure.Auth;

public interface ITokenValidationParametersFactory
{
    TokenValidationParameters CreateValidationParameters();
}
