using System.Security.Cryptography;
using VcsDevelop.Application.VcsObjects.Services;

namespace VcsDevelop.Infrastructure.Services;

public sealed class HashService : IHashService
{
    public async Task<string> ComputeSha256Async(Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        using var sha256 = SHA256.Create();

        var buffer = new byte[81920];
        int read;

        while ((read = await stream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            sha256.TransformBlock(buffer, 0, read, null, 0);
        }

        sha256.TransformFinalBlock([], 0, 0);

        return Convert.ToHexStringLower(sha256.Hash!);
    }
}
