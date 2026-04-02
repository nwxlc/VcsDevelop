using GitDevelop.Application.Accounts.Auth;
using GitDevelop.Core.Application;
using GitDevelop.Infrastructure.Auth;
using GitDevelop.Infrastructure.Options.Tokens;
using GitDevelop.WebApi.Contexts;
using Microsoft.Extensions.Options;

namespace GitDevelop.WebApi.Extensions;

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

        services.AddSingleton<RequestContext>();
        services.AddSingleton<IRequestContext>(sp => sp.GetRequiredService<RequestContext>());

        return services;
    }
}
