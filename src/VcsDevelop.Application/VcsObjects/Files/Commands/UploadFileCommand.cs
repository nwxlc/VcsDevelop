namespace VcsDevelop.Application.VcsObjects.Files.Commands;

public sealed class UploadFileCommand
{
    public Stream Stream { get; private init; }
    public string FileName { get; private init; }

    private UploadFileCommand(
        Stream stream,
        string fileName)
    {
        Stream = stream;
        FileName = fileName;
    }

    public static UploadFileCommand Create(
        Stream stream,
        string fileName)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        return new UploadFileCommand(stream, fileName);
    }
}
