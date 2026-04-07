namespace GitDevelop.Domain.GitObjects;

public sealed class ContentHash
{
    public byte[] Value { get; private init; }

    public ContentHash(byte[] value)
    {
        Value = value;
    }
}
