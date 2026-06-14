using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects.Errors;

namespace VcsDevelop.Application.VcsObjects.Documents.QueryHandlers;

public sealed class GetDocumentLogHandler : IGetDocumentLogHandler
{
    private readonly IRequestContext _requestContext;
    private readonly IDocumentRepository _documentRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ICommitRepository _commitRepository;

    public GetDocumentLogHandler(
        IRequestContext requestContext,
        IDocumentRepository documentRepository,
        IBranchRepository branchRepository,
        ICommitRepository commitRepository)
    {
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(branchRepository);

        _requestContext = requestContext;
        _documentRepository = documentRepository;
        _branchRepository = branchRepository;
        _commitRepository = commitRepository;
    }

    public async Task<DocumentLogResponse> HandleAsync(GetDocumentLogQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var accountId = _requestContext.GetRequiredAccountId();

        var document = await _documentRepository
            .FindByIdAsync(request.DocumentId, accountId, cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new DocumentNotFound(request.DocumentId);
        }

        var branch = await _branchRepository
            .FindByDocumentAndNameAsync(
                document.Id,
                document.DefaultBranchName,
                cancellationToken)
            .ConfigureAwait(false);

        if (branch is null)
        {
            return new DocumentLogResponse
            {
                DocumentId = document.Id,
                BranchName = document.DefaultBranchName,
                Commits = []
            };
        }

        var commits = new List<CommitLogItemResponse>();
        var currentCommitId = branch.HeadCommitId;
        var visitedCommitIds = new HashSet<string>(StringComparer.Ordinal);

        while (!string.IsNullOrWhiteSpace(currentCommitId))
        {
            if (!visitedCommitIds.Add(currentCommitId))
            {
                break;
            }

            var commit = await _commitRepository
                .FindByIdAsync(currentCommitId, cancellationToken)
                .ConfigureAwait(false);

            if (commit is null)
            {
                break;
            }

            var parentIds = commit.ParentIds
                .Select(parent => parent.ParentId)
                .ToArray();

            commits.Add(new CommitLogItemResponse
            {
                Id = commit.Id,
                TreeId = commit.RootTreeId,
                Message = commit.Message.Value,
                AccountId = commit.AccountId,
                CreatedAt = commit.CreatedAt,
                ParentIds = parentIds
            });

            currentCommitId = parentIds.FirstOrDefault();
        }

        return new DocumentLogResponse
        {
            DocumentId = document.Id,
            BranchName = branch.Name,
            Commits = commits
        };
    }
}
