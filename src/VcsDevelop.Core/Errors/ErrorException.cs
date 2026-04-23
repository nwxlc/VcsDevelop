namespace VcsDevelop.Core.Errors;

public sealed class ErrorException : Exception
{
    public Error Error { get; }
    public bool HasDetails { get; }

    public ErrorException(Error error, string? details = null)
        : base(details ?? error.Message)
    {
        ArgumentNullException.ThrowIfNull(error);

        Error = error;
        HasDetails = details is not null;
    }
}
