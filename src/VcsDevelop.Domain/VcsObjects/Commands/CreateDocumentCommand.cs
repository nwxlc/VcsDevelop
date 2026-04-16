namespace VcsDevelop.Domain.VcsObjects.Commands;

public sealed class CreateDocumentCommand
{
    public string Name { get; private init; }
    public string DefaultBranchName { get; private init; }
    public string? Description { get; private init; }
    public List<string>? Tags { get; private init; }

    private CreateDocumentCommand(
        string name,
        string defaultBranchName,
        string? description,
        List<string>? tags)
    {
        Name = name;
        DefaultBranchName = defaultBranchName;
        Description = description;
        Tags = tags;
    }

    public static CreateDocumentCommand Create(
        string name,
        string defaultBranchName,
        string? description,
        List<string>? tags)
    {
        return new CreateDocumentCommand(
            name,
            defaultBranchName,
            description,
            tags);
    }
}
