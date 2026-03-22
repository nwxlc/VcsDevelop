namespace GitDevelop.Domain.Entities;

public sealed class RepositoryTag
{
    public string Value { get; private init; }

    public RepositoryTag(string value)
    {
        Value = value;
    }
}
