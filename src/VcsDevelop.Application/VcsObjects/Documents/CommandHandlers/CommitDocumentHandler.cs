using System.Security.Cryptography;
using System.Text;
using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects;
using VcsDevelop.Domain.VcsObjects.Commands;
using VcsDevelop.Domain.VcsObjects.Errors;

namespace VcsDevelop.Application.VcsObjects.Documents.CommandHandlers;

public sealed class CommitDocumentHandler : ICommitDocumentHandler
{
    private readonly IBranchRepository _branchRepository;
    private readonly ICommitRepository _commitRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IRequestContext _requestContext;
    private readonly IStagingAreaRepository _stagingAreaRepository;
    private readonly ITreeRepository _treeRepository;

    public CommitDocumentHandler(
        IDocumentRepository documentRepository,
        IBranchRepository branchRepository,
        ICommitRepository commitRepository,
        ITreeRepository treeRepository,
        IStagingAreaRepository stagingAreaRepository,
        IRequestContext requestContext)
    {
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(branchRepository);
        ArgumentNullException.ThrowIfNull(commitRepository);
        ArgumentNullException.ThrowIfNull(treeRepository);
        ArgumentNullException.ThrowIfNull(stagingAreaRepository);
        ArgumentNullException.ThrowIfNull(requestContext);

        _documentRepository = documentRepository;
        _branchRepository = branchRepository;
        _commitRepository = commitRepository;
        _treeRepository = treeRepository;
        _stagingAreaRepository = stagingAreaRepository;
        _requestContext = requestContext;
    }

    public async Task<CommitDocumentResponse> HandleAsync(
        CommitDocumentCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Message);

        var accountId = _requestContext.GetRequiredAccountId();

        var document = await GetDocumentAsync(request.DocumentId, accountId, cancellationToken)
            .ConfigureAwait(false);

        var stagedEntries = await GetStagedEntriesAsync(document.Id, accountId, cancellationToken)
            .ConfigureAwait(false);

        var branch = await _branchRepository
            .FindByDocumentAndNameAsync(document.Id, document.DefaultBranchName, cancellationToken)
            .ConfigureAwait(false);

        var tree = await StoreTreeAsync(branch, stagedEntries, cancellationToken).ConfigureAwait(false);

        var commit = await StoreCommitAsync(
                document,
                branch,
                tree,
                accountId,
                request.Message,
                cancellationToken)
            .ConfigureAwait(false);

        branch = await StoreBranchAsync(document, branch, commit.Id, cancellationToken).ConfigureAwait(false);
        await _stagingAreaRepository.ClearAsync(document.Id, accountId, cancellationToken).ConfigureAwait(false);

        return new CommitDocumentResponse
        {
            DocumentId = document.Id,
            BranchName = branch.Name,
            CommitId = commit.Id,
            TreeId = tree.Id,
            FilesCommitted = stagedEntries.Count
        };
    }

    private async Task<Document> GetDocumentAsync(
        Guid documentId,
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository
            .FindByIdAsync(documentId, accountId, cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new DocumentNotFound(documentId);
        }

        return document;
    }

    private async Task<IReadOnlyCollection<StagedFileEntry>> GetStagedEntriesAsync(
        Guid documentId,
        Guid accountId,
        CancellationToken cancellationToken)
    {
        var stagedEntries = await _stagingAreaRepository
            .GetAllAsync(documentId, accountId, cancellationToken)
            .ConfigureAwait(false);

        if (stagedEntries.Count == 0)
        {
            throw new InvalidOperationException("Staging Area is empty.");
        }

        return stagedEntries;
    }

    private async Task<Tree> StoreTreeAsync(
        Branch? branch,
        IReadOnlyCollection<StagedFileEntry> stagedEntries,
        CancellationToken cancellationToken)
    {
        var treeEntries = await BuildTreeEntriesAsync(branch, stagedEntries, cancellationToken)
            .ConfigureAwait(false);
        var tree = Tree.Create(ComputeTreeId(treeEntries), treeEntries);

        await _treeRepository.SetAsync(tree, cancellationToken).ConfigureAwait(false);

        return tree;
    }

    private async Task<Commit> StoreCommitAsync(
        Document document,
        Branch? branch,
        Tree tree,
        Guid accountId,
        string message,
        CancellationToken cancellationToken)
    {
        var commit = CreateCommit(document, branch, tree.Id, accountId, message);

        await _commitRepository.SetAsync(commit, cancellationToken).ConfigureAwait(false);

        return commit;
    }

    private async Task<Branch> StoreBranchAsync(
        Document document,
        Branch? branch,
        string commitId,
        CancellationToken cancellationToken)
    {
        branch = CreateOrUpdateBranch(document, branch, commitId);

        await _branchRepository.SetAsync(branch, cancellationToken).ConfigureAwait(false);

        return branch;
    }

    private static Commit CreateCommit(
        Document document,
        Branch? branch,
        string treeId,
        Guid accountId,
        string message)
    {
        var parentIds = branch is null ? Array.Empty<string>() : [branch.HeadCommitId];

        return Commit.Create(
            document.Id,
            treeId,
            parentIds,
            accountId,
            CommitMessage.Create(message));
    }

    private static Branch CreateOrUpdateBranch(
        Document document,
        Branch? branch,
        string commitId)
    {
        if (branch is null)
        {
            return Branch.Create(
                document.Id,
                document.DefaultBranchName,
                commitId);
        }

        branch.UpdateHeadCommit(commitId);
        return branch;
    }

    private async Task<IReadOnlyCollection<TreeEntry>> BuildTreeEntriesAsync(
        Branch? branch,
        IReadOnlyCollection<StagedFileEntry> stagedEntries,
        CancellationToken cancellationToken)
    {
        var entries = new Dictionary<string, string>(StringComparer.Ordinal);

        if (branch is not null)
        {
            var headCommit = await _commitRepository.FindByIdAsync(branch.HeadCommitId, cancellationToken)
                .ConfigureAwait(false);

            if (headCommit is not null)
            {
                var currentTree = await _treeRepository.FindByIdAsync(headCommit.RootTreeId, cancellationToken)
                    .ConfigureAwait(false);

                if (currentTree is not null)
                {
                    foreach (var entry in currentTree.Entries)
                    {
                        entries[entry.Name] = entry.ObjectId;
                    }
                }
            }
        }

        foreach (var stagedEntry in stagedEntries)
        {
            entries[stagedEntry.RepositoryPath] = stagedEntry.BlobId;
        }

        return entries
            .OrderBy(item => item.Key, StringComparer.Ordinal)
            .Select(item => new TreeEntry(item.Key, item.Value))
            .ToArray();
    }

    private static string ComputeTreeId(IReadOnlyCollection<TreeEntry> entries)
    {
        var payload = string.Join('\n', entries.Select(entry => $"{entry.Name}:{entry.ObjectId}"));
        return ComputeSha256(Encoding.UTF8.GetBytes(payload));
    }

    private static string ComputeSha256(byte[] data)
    {
        return Convert.ToHexStringLower(SHA256.HashData(data));
    }
}
