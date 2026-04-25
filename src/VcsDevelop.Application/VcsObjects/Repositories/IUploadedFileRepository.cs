using VcsDevelop.Application.VcsObjects.Models;

namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface IUploadedFileRepository
{
    Task<UploadedFileReference?> FindByIdAsync(Guid uploadId, CancellationToken cancellationToken = default);

    Task SetAsync(UploadedFileReference reference, CancellationToken cancellationToken = default);

    Task RemoveAsync(Guid uploadId, CancellationToken cancellationToken = default);
}
