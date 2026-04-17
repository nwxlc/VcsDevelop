namespace VcsDevelop.Domain.VcsObjects;

public sealed class DocumentMetadata
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public IReadOnlyCollection<DocumentTag>? Tags { get; set; }

    // EF only
    private DocumentMetadata()
    {
        Tags = new HashSet<DocumentTag>();
    }

    private DocumentMetadata(
        string title,
        string? description,
        IReadOnlyCollection<string>? tags)
    {
        Title = title;
        Description = description;
        Tags = tags?.Select(tag => new DocumentTag(tag)).ToArray() ?? [];
    }

    public static DocumentMetadata Create(
        string title,
        string? description,
        IReadOnlyCollection<string>? tags)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new DocumentMetadata(title, description, tags);
    }
}
