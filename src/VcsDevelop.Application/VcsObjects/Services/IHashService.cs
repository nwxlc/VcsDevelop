namespace VcsDevelop.Application.VcsObjects.Services;

public interface IHashService
{
    Task<string> ComputeSha1Async(
        Stream stream,
        CancellationToken cancellationToken);
}
