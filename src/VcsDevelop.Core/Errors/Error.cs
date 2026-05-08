namespace VcsDevelop.Core.Errors;

public abstract class Error
{
    public abstract string Message { get; }

    public static implicit operator ErrorException(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new ErrorException(error, error.Message);
    }

    public virtual string BuildType()
        => GetType().Name.ToLowerInvariant();

    public ErrorException WithDetails(
        string? details = null)
    {
        return new ErrorException(this, details);
    }
}
