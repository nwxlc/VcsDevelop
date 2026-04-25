using VcsDevelop.Application.VcsObjects.Models;

namespace VcsDevelop.Application.VcsObjects.Services;

public interface ICompressionService
{
    Task<CompressionResult> CompressAsync(
        Stream input,
        CancellationToken cancellationToken);

    Task DecompressAsync(
        Stream input,
        Stream output,
        CancellationToken cancellationToken);
}
