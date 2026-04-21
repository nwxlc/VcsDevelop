using Commit = VcsDevelop.Domain.VcsObjects.Commit;

namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface ICommitRepository
{
    Task<Commit?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    Task SetAsync(Commit commit, CancellationToken cancellationToken = default);
}
