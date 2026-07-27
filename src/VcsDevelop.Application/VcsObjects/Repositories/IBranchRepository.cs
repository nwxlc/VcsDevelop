using Branch = VcsDevelop.Domain.VcsObjects.Branch;

namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface IBranchRepository
{
    Task<Branch?> FindByDocumentAndNameAsync(
        Guid documentId,
        string branchName,
        CancellationToken cancellationToken = default);

    void Add(Branch branch);
}
