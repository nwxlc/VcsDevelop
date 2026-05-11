namespace VcsDevelop.Domain.VcsObjects;

public sealed class CommitMessage
{
    public string Value { get; private init; }

    public CommitMessage(string value)
    {
        Value = value;
    }

    public static CommitMessage Create(string value) => new(value);
}
