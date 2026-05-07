namespace VcsDevelop.WebApi.Contracts.VcsObjects;

public sealed class UploadFileRequest
{
    public required IFormFile File { get; init; }
}
