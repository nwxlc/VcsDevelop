namespace VcsDevelop.Application.VcsObjects.Files.Models;

public sealed class DownloadFileResponse
{
    public Stream Stream { get; init; }
    public string FileName { get; init; }
    public string ContentType { get; init; }

    private DownloadFileResponse(
        Stream stream,
        string fileName,
        string contentType)
    {
        Stream = stream;
        FileName = fileName;
        ContentType = contentType;
    }

    public static DownloadFileResponse Create(Stream stream, string fileName, string contentType)
    {
        return new DownloadFileResponse(
            stream,
            fileName,
            contentType);
    }
}
