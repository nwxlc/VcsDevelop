namespace GitDevelop.Domain.Entities;

public sealed class RepositoryMetadata
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IReadOnlyCollection<string> Tags { get; set; }

    public RepositoryMetadata(
        string title,
        string description,
        IReadOnlyCollection<string> tags)
    {
        Title = title;
        Description = description;
        Tags = tags;
    }
}
