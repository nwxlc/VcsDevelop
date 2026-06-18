using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects;

namespace VcsDevelop.Application.VcsObjects.Documents.QueryHandlers;

public sealed class GetRepositoryTreeHandler : IGetRepositoryTreeHandler
{
    private readonly IBranchRepository _branchRepository;
    private readonly ICommitRepository _commitRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IRequestContext _requestContext;
    private readonly ITreeRepository _treeRepository;

    public GetRepositoryTreeHandler(
        IDocumentRepository documentRepository,
        IBranchRepository branchRepository,
        ICommitRepository commitRepository,
        ITreeRepository treeRepository,
        IRequestContext requestContext)
    {
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(branchRepository);
        ArgumentNullException.ThrowIfNull(commitRepository);
        ArgumentNullException.ThrowIfNull(treeRepository);
        ArgumentNullException.ThrowIfNull(requestContext);

        _documentRepository = documentRepository;
        _branchRepository = branchRepository;
        _commitRepository = commitRepository;
        _treeRepository = treeRepository;
        _requestContext = requestContext;
    }

    public async Task<RepositoryTreeResponse> HandleAsync(
        GetRepositoryTreeQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var accountId = _requestContext.GetRequiredAccountId();
        var document = await _documentRepository
            .FindByIdAsync(request.DocumentId, accountId, cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new KeyNotFoundException($"Document '{request.DocumentId}' was not found.");
        }

        var normalizedPath = NormalizeRepositoryPath(request.Path);
        var headState = await GetHeadStateAsync(document.Id, document.DefaultBranchName, cancellationToken)
            .ConfigureAwait(false);
        var entries = headState.Tree is null
            ? []
            : BuildEntries(headState.Tree.Entries, normalizedPath);

        if (entries.Count > 0 && headState.Commit is not null)
        {
            await PopulateLastCommitMessagesAsync(
                    entries,
                    headState.Commit,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        return new RepositoryTreeResponse
        {
            DocumentId = document.Id,
            BranchName = document.DefaultBranchName,
            Path = normalizedPath,
            Entries = entries
        };
    }

    private async Task<(Commit? Commit, Tree? Tree)> GetHeadStateAsync(
        Guid documentId,
        string branchName,
        CancellationToken cancellationToken)
    {
        var branch = await _branchRepository
            .FindByDocumentAndNameAsync(documentId, branchName, cancellationToken)
            .ConfigureAwait(false);

        if (branch is null)
        {
            return (null, null);
        }

        var commit = await _commitRepository.FindByIdAsync(branch.HeadCommitId, cancellationToken)
            .ConfigureAwait(false);
        if (commit is null)
        {
            return (null, null);
        }

        var tree = await _treeRepository.FindByIdAsync(commit.RootTreeId, cancellationToken)
            .ConfigureAwait(false);
        return (commit, tree);
    }

    private static IReadOnlyCollection<RepositoryTreeEntryResponse> BuildEntries(
        IReadOnlyCollection<TreeEntry> treeEntries,
        string normalizedPath)
    {
        var prefix = string.IsNullOrWhiteSpace(normalizedPath)
            ? string.Empty
            : normalizedPath + "/";

        var directoryNames = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<RepositoryTreeEntryResponse>();

        foreach (var entry in treeEntries.OrderBy(item => item.Name, StringComparer.Ordinal))
        {
            if (!entry.Name.StartsWith(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            var remainder = entry.Name[prefix.Length..];
            if (string.IsNullOrWhiteSpace(remainder))
            {
                continue;
            }

            var separatorIndex = remainder.IndexOf('/');
            if (separatorIndex < 0)
            {
                result.Add(new RepositoryTreeEntryResponse
                {
                    Name = remainder,
                    Path = entry.Name,
                    Type = "file",
                    BlobId = entry.ObjectId
                });

                continue;
            }

            var directoryName = remainder[..separatorIndex];
            if (!directoryNames.Add(directoryName))
            {
                continue;
            }

            result.Add(new RepositoryTreeEntryResponse
            {
                Name = directoryName,
                Path = string.IsNullOrWhiteSpace(prefix)
                    ? directoryName
                    : prefix + directoryName,
                Type = "directory"
            });
        }

        return result;
    }

    private async Task PopulateLastCommitMessagesAsync(
        IReadOnlyCollection<RepositoryTreeEntryResponse> entries,
        Commit headCommit,
        CancellationToken cancellationToken)
    {
        var entryMessages = entries.ToDictionary(
            entry => entry.Path,
            _ => (string?)null,
            StringComparer.Ordinal);

        var treeCache = new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.Ordinal);
        var currentCommit = headCommit;
        var visitedCommitIds = new HashSet<string>(StringComparer.Ordinal);

        while (true)
        {
            if (!visitedCommitIds.Add(currentCommit.Id))
            {
                break;
            }

            var currentTree = await GetTreeStateAsync(currentCommit.RootTreeId, treeCache, cancellationToken)
                .ConfigureAwait(false);

            Commit? parentCommit = null;
            IReadOnlyDictionary<string, string> parentTree = new Dictionary<string, string>(StringComparer.Ordinal);

            var parentId = currentCommit.ParentIds
                .Select(parent => parent.ParentId)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(parentId))
            {
                parentCommit = await _commitRepository.FindByIdAsync(parentId, cancellationToken)
                    .ConfigureAwait(false);

                if (parentCommit is not null)
                {
                    parentTree = await GetTreeStateAsync(parentCommit.RootTreeId, treeCache, cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            var changedPaths = GetChangedPaths(currentTree, parentTree);
            if (changedPaths.Count > 0)
            {
                foreach (var entry in entries)
                {
                    if (entryMessages[entry.Path] is not null)
                    {
                        continue;
                    }

                    if (!TouchesEntry(entry, changedPaths))
                    {
                        continue;
                    }

                    entryMessages[entry.Path] = currentCommit.Message.Value;
                }
            }

            if (parentCommit is null)
            {
                break;
            }

            currentCommit = parentCommit;
        }

        foreach (var entry in entries)
        {
            entry.LastCommitMessage = entryMessages[entry.Path];
        }
    }

    private async Task<IReadOnlyDictionary<string, string>> GetTreeStateAsync(
        string treeId,
        IDictionary<string, IReadOnlyDictionary<string, string>> treeCache,
        CancellationToken cancellationToken)
    {
        if (treeCache.TryGetValue(treeId, out var cachedTree))
        {
            return cachedTree;
        }

        var tree = await _treeRepository.FindByIdAsync(treeId, cancellationToken).ConfigureAwait(false);
        var state = tree is null
            ? new Dictionary<string, string>(StringComparer.Ordinal)
            : tree.Entries.ToDictionary(entry => entry.Name, entry => entry.ObjectId, StringComparer.Ordinal);

        treeCache[treeId] = state;
        return state;
    }

    private static HashSet<string> GetChangedPaths(
        IReadOnlyDictionary<string, string> currentTree,
        IReadOnlyDictionary<string, string> parentTree)
    {
        var changedPaths = new HashSet<string>(StringComparer.Ordinal);

        foreach (var (path, objectId) in currentTree)
        {
            if (!parentTree.TryGetValue(path, out var parentObjectId) ||
                !string.Equals(parentObjectId, objectId, StringComparison.Ordinal))
            {
                changedPaths.Add(path);
            }
        }

        foreach (var path in parentTree.Keys)
        {
            if (!currentTree.ContainsKey(path))
            {
                changedPaths.Add(path);
            }
        }

        return changedPaths;
    }

    private static bool TouchesEntry(
        RepositoryTreeEntryResponse entry,
        HashSet<string> changedPaths)
    {
        if (entry.Type == "file")
        {
            return changedPaths.Contains(entry.Path);
        }

        var prefix = entry.Path + "/";
        return changedPaths.Any(path => path.StartsWith(prefix, StringComparison.Ordinal));
    }

    private static string NormalizeRepositoryPath(string? repositoryPath)
    {
        if (string.IsNullOrWhiteSpace(repositoryPath))
        {
            return string.Empty;
        }

        var normalizedPath = repositoryPath
            .Replace('\\', '/')
            .Trim();

        normalizedPath = normalizedPath.Trim('/');

        ValidatePathSegments(normalizedPath);

        return normalizedPath;
    }

    private static void ValidatePathSegments(string repositoryPath)
    {
        foreach (var segment in repositoryPath.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (segment is "." or "..")
            {
                throw new ArgumentException("Repository path contains invalid segments.", nameof(repositoryPath));
            }
        }
    }
}
