using Serilog;

namespace VcsDevelop.WebApi.Extensions;

public static class AddSerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog();

        return builder;
    }
}
