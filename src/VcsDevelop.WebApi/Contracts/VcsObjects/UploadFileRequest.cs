namespace VcsDevelop.WebApi.Contracts.VcsObjects;

public sealed class UploadFileRequest
{
    public IFormFile File { get; init; }
}
