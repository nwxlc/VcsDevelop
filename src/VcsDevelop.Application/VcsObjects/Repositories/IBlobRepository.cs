using Blob = VcsDevelop.Domain.VcsObjects.Blob;

namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface IBlobRepository
{
    Task<Blob?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    Task SetAsync(Blob blob, CancellationToken cancellationToken = default);

    Task RemoveAsync(string id, CancellationToken cancellationToken = default);
}
