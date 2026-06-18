namespace VcsDevelop.Application.VcsObjects.Services;

public interface IHashService
{
    Task<string> ComputeSha256Async(
        Stream stream,
        CancellationToken cancellationToken);
}
