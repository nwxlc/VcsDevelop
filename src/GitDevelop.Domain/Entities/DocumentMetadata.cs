namespace GitDevelop.Domain.Entities;

public sealed class DocumentMetadata
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IReadOnlyCollection<string> Tags { get; set; }

    public DocumentMetadata(
        string title,
        string description,
        IReadOnlyCollection<string> tags)
    {
        Title = title;
        Description = description;
        Tags = tags;
    }
}
