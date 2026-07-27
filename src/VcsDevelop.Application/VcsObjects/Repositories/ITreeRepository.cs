using Tree = VcsDevelop.Domain.VcsObjects.Tree;

namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface ITreeRepository
{
    Task<Tree?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    void Add(Tree tree);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}
