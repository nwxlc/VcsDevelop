namespace VcsDevelop.Application.VcsObjects.Files.Commands;

public sealed class UploadFileCommand
{
    public Guid DocumentId { get; private init; }
    public Stream Stream { get; private init; }
    public string FileName { get; private init; }

    private UploadFileCommand(
        Guid documentId,
        Stream stream,
        string fileName)
    {
        Stream = stream;
        FileName = fileName;
        DocumentId = documentId;
    }

    public static UploadFileCommand Create(
        Guid documentId,
        Stream stream,
        string fileName)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        return new UploadFileCommand(documentId, stream, fileName);
    }
}
