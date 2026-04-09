namespace VcsDevelop.Domain.VcsObjects;

public sealed class CommitMessage
{
    public string Value { get; private init; }

    public CommitMessage(string value)
    {
        Value = value;
    }
}
