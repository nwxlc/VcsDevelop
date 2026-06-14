using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects;
using VcsDevelop.Domain.VcsObjects.Commands;
using VcsDevelop.Domain.VcsObjects.Errors;

namespace VcsDevelop.Application.Accounts.CommandHandlers;

public sealed class RevertDocumentHandler : IRevertDocumentHandler
{
    private readonly IRequestContext _requestContext;
    private readonly IDocumentRepository _documentRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ICommitRepository _commitRepository;

    public RevertDocumentHandler(
        IRequestContext requestContext,
        IDocumentRepository documentRepository,
        IBranchRepository branchRepository,
        ICommitRepository commitRepository)
    {
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(branchRepository);
        ArgumentNullException.ThrowIfNull(commitRepository);

        _requestContext = requestContext;
        _documentRepository = documentRepository;
        _branchRepository = branchRepository;
        _commitRepository = commitRepository;
    }

    public async Task<RevertDocumentResponse> HandleAsync(RevertDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var accountId = _requestContext.GetRequiredAccountId();

        var document = await _documentRepository.FindByIdAsync(
                request.DocumentId,
                accountId,
                cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new DocumentNotFound(request.DocumentId);
        }

        var branch = await _branchRepository.FindByDocumentAndNameAsync(
                document.Id,
                request.BranchName,
                cancellationToken)
            .ConfigureAwait(false);

        if (branch is null)
        {
            throw new KeyNotFoundException($"Branch '{document.DefaultBranchName}' was not found.");
        }

        var headCommit = await _commitRepository
            .FindByIdAsync(branch.HeadCommitId, cancellationToken)
            .ConfigureAwait(false);

        if (headCommit is null)
        {
            throw new KeyNotFoundException($"Commit '{branch.HeadCommitId}' was not found.");
        }

        var targetCommit = await _commitRepository
            .FindByIdAsync(request.CommitId, cancellationToken)
            .ConfigureAwait(false);

        if (targetCommit is null)
        {
            throw new KeyNotFoundException($"Commit '{request.CommitId}' was not found.");
        }

        var revertCommit = Commit.Create(
            document.Id,
            targetCommit.RootTreeId,
            [headCommit.Id],
            accountId,
            CommitMessage.Create($"Revert to {targetCommit.Id}"));

        await _commitRepository
            .SetAsync(revertCommit, cancellationToken)
            .ConfigureAwait(false);

        branch.UpdateHeadCommit(revertCommit.Id);

        await _branchRepository
            .SetAsync(branch, cancellationToken)
            .ConfigureAwait(false);

        return new RevertDocumentResponse
        {
            DocumentId = document.Id,
            BranchName = branch.Name,
            RevertedToCommitId = targetCommit.Id,
            NewCommitId = revertCommit.Id,
            TreeId = revertCommit.RootTreeId
        };
    }
}
