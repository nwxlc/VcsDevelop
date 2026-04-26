using Tree = VcsDevelop.Domain.VcsObjects.Tree;

namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface ITreeRepository
{
    Task<Tree?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    Task SetAsync(Tree tree, CancellationToken cancellationToken = default);
}
