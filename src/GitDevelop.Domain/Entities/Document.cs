namespace GitDevelop.Domain.Entities;

public class Document
{
    public Guid Id { get; private init; }
    public string Name { get; set; }
    public string DefaultBrancName { get; set; }
    public DocumentMetadata Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    
}
