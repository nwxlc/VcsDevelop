using Microsoft.AspNetCore.Mvc;
using VcsDevelop.Core.Errors;
using Conflict = VcsDevelop.Core.Errors.Conflict;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

namespace VcsDevelop.WebApi.Extensions;

internal static class ProblemDetailsExtensions
{
    private const string ErrorKey = "error";
    private const string CodeKey = "code";

    public static void Configure(ProblemDetailsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.IncludeExceptionDetails = (ctx, _) =>
            !ctx.RequestServices
                .GetRequiredService<IWebHostEnvironment>()
                .IsProduction();

        options.OnBeforeWriteDetails = (ctx, details) => { details.Instance = ctx.Request.Path; };

        options.Map<ErrorException>((_, exception) =>
        {
            var error = exception.Error;
            var status = MapStatus(error);

            var problem = new ProblemDetails
            {
                Type = BuildType("VCSDevelop", error),
                Title = error.Message,
                Status = status,
                Detail = exception.HasDetails ? exception.Message : null,
                Extensions =
                {
                    [CodeKey] = error.BuildType(),
                    [ErrorKey] = error
                }
            };

            return problem;
        });

        options.Map<UnauthorizedAccessException>(ex =>
        {
            return new ProblemDetails
            {
                Type = "about:blank",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = ex.Message
            };
        });

        options.Map<Exception>(ex =>
        {
            return new ProblemDetails
            {
                Type = "about:blank",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message
            };
        });
    }

    private static int MapStatus(Error error) => error switch
    {
        Conflict => StatusCodes.Status409Conflict,
        NotFound => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string BuildType(string serviceName, Error error)
    {
        return $"/{serviceName}/api/errors/{error.BuildType()}";
    }
}
