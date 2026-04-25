using System.IO.Compression;
using VcsDevelop.Application.VcsObjects.Models;
using VcsDevelop.Application.VcsObjects.Services;

namespace VcsDevelop.Infrastructure.Services;

public sealed class ZLibCompressionService : ICompressionService
{
    public async Task<CompressionResult> CompressAsync(Stream input, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);

        var tempPath = Path.GetTempFileName();

        await using (var file = File.Create(tempPath))
        await using (var zlib = new ZLibStream(file, CompressionLevel.Optimal))
        {
            if (input.CanSeek)
            {
                input.Position = 0;
            }

            await input.CopyToAsync(zlib, cancellationToken);
        }

        var resultStream = File.OpenRead(tempPath);

        return CompressionResult.Create(resultStream, resultStream.Length, tempPath);
    }

    public async Task DecompressAsync(Stream input, Stream output, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);

        if (input.CanSeek)
        {
            input.Position = 0;
        }

        await using var zlib = new ZLibStream(input, CompressionMode.Decompress, leaveOpen: true);

        await zlib.CopyToAsync(output, cancellationToken);
    }
}
