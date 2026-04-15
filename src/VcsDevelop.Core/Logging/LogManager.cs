using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using VcsDevelop.Core.Logging.Extensions;

namespace VcsDevelop.Core.Logging;

public static class LogManager
{
    private static volatile ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

    public static void Initialize(ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _loggerFactory = _loggerFactory is NullLoggerFactory
            ? loggerFactory
            : throw new InvalidOperationException("LogManager has already been configured.");
    }

    public static ILogger CreateLogger<TSource>()
    {
        return CreateLogger(typeof(TSource).DisplayName());
    }

    public static ILogger CreateLogger(string loggerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loggerName);

        return _loggerFactory.CreateLogger(loggerName);
    }
}
