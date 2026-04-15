using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;

namespace VcsDevelop.WebApi.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Введите ваш JWT токен"
                };

                document.Security =
                [
                    new OpenApiSecurityRequirement
                        { [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = [] }
                ];

                return Task.CompletedTask;
            });
        });
        
        return services;
    }
}
