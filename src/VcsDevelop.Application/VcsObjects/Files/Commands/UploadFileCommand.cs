namespace VcsDevelop.Application.VcsObjects.Files.Commands;

public sealed class UploadFileCommand
{
    public Guid DocumentId { get; private init; }
    public Stream Stream { get; private init; }
    public string FileName { get; private init; }
    public string? Path { get; private init; }

    private UploadFileCommand(
        Guid documentId,
        Stream stream,
        string fileName,
        string? path = null)
    {
        Stream = stream;
        FileName = fileName;
        DocumentId = documentId;
        Path = path;
    }

    public static UploadFileCommand Create(
        Guid documentId,
        Stream stream,
        string fileName,
        string? path = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        return new UploadFileCommand(documentId, stream, fileName, path);
    }
}
