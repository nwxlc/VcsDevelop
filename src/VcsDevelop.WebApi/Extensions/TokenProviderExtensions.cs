using Microsoft.Extensions.Options;
using VcsDevelop.Application.Accounts.Auth;
using VcsDevelop.Core.Application;
using VcsDevelop.Infrastructure.Auth;
using VcsDevelop.Infrastructure.Options.Tokens;
using VcsDevelop.WebApi.Contexts;

namespace VcsDevelop.WebApi.Extensions;

public static class TokenProviderExtensions
{
    public static IServiceCollection AddTokenProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<TokenOption>().Bind(configuration.GetSection(TokenOption.Name));
        
        services.AddSingleton<ITokenSettings>(provider =>
            provider.GetRequiredService<IOptions<TokenOption>>().Value);
        
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<ITokenValidationParametersFactory, TokenValidationParametersFactory>();
        services.AddSingleton<IRefreshTokenProvider, RefreshTokenProvider>();

        services.AddSingleton<RequestContext>();
        services.AddSingleton<IRequestContext>(sp => sp.GetRequiredService<RequestContext>());

        return services;
    }
}
