namespace GitDevelop.Domain.GitObjects;

public sealed class CommitMessage
{
    public string Value { get; private init; }

    public CommitMessage(string value)
    {
        Value = value;
    }
}
