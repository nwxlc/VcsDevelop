using Commit = VcsDevelop.Domain.VcsObjects.Commit;

namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface ICommitRepository
{
    Task<Commit?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    void Add(Commit commit);
}
