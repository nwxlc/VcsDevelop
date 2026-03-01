namespace GitDevelop.Domain.Entities;

public class Blob
{
    public Guid Id { get; set; }
    public ContentHash Hash { get; set; }
    public long Size { get; set; }
    public DateTime CreateAt { get; set; }
}
