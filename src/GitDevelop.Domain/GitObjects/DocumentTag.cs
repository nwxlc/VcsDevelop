namespace GitDevelop.Domain.GitObjects;

public sealed class DocumentTag
{
    public string Value { get; private init; }

    public DocumentTag(string value)
    {
        Value = value;
    }
}
