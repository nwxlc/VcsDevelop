namespace GitDevelop.Domain.GitObjects;

public sealed class DocumentMetadata
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IReadOnlyCollection<DocumentTag> Tags { get; set; }

    // EF only
    private DocumentMetadata()
    {
        Tags = new HashSet<DocumentTag>();
    }

    public DocumentMetadata(
        string title,
        string description,
        IReadOnlyCollection<string> tags)
    {
        Title = title;
        Description = description;
        Tags = tags.Select(tag => new DocumentTag(tag)).ToArray();
    }
}
