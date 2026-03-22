namespace GitDevelop.Domain.Entities;

public sealed class RepositoryMetadata
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IReadOnlyCollection<RepositoryTag> Tags { get; set; }

    public RepositoryMetadata(
        string title,
        string description,
        IReadOnlyCollection<string> tags)
    {
        Title = title;
        Description = description;
        Tags = tags.Select(tag => new RepositoryTag(tag)).ToArray();
    }
}
