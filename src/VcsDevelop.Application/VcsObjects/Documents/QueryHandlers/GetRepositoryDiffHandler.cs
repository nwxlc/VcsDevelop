using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects;

namespace VcsDevelop.Application.VcsObjects.Documents.QueryHandlers;

public sealed class GetRepositoryDiffHandler : IGetRepositoryDiffHandler
{
    private readonly ICommitRepository _commitRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IRequestContext _requestContext;
    private readonly ITreeRepository _treeRepository;

    public GetRepositoryDiffHandler(
        IDocumentRepository documentRepository,
        ICommitRepository commitRepository,
        ITreeRepository treeRepository,
        IRequestContext requestContext)
    {
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(commitRepository);
        ArgumentNullException.ThrowIfNull(treeRepository);
        ArgumentNullException.ThrowIfNull(requestContext);

        _documentRepository = documentRepository;
        _commitRepository = commitRepository;
        _treeRepository = treeRepository;
        _requestContext = requestContext;
    }

    public async Task<RepositoryDiffResponse> HandleAsync(
        GetRepositoryDiffQuery request,
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

        var fromCommit = await _commitRepository.FindByIdAsync(request.FromCommitId, cancellationToken)
            .ConfigureAwait(false);
        if (fromCommit is null)
        {
            throw new KeyNotFoundException($"Commit '{request.FromCommitId}' was not found.");
        }

        var toCommit = await _commitRepository.FindByIdAsync(request.ToCommitId, cancellationToken)
            .ConfigureAwait(false);
        if (toCommit is null)
        {
            throw new KeyNotFoundException($"Commit '{request.ToCommitId}' was not found.");
        }

        var fromTree = await _treeRepository.FindByIdAsync(fromCommit.RootTreeId, cancellationToken)
            .ConfigureAwait(false);
        if (fromTree is null)
        {
            throw new KeyNotFoundException($"Tree '{fromCommit.RootTreeId}' was not found.");
        }

        var toTree = await _treeRepository.FindByIdAsync(toCommit.RootTreeId, cancellationToken)
            .ConfigureAwait(false);
        if (toTree is null)
        {
            throw new KeyNotFoundException($"Tree '{toCommit.RootTreeId}' was not found.");
        }

        var normalizedPath = NormalizeRepositoryPath(request.Path);
        var entries = BuildDiff(fromTree.Entries, toTree.Entries, normalizedPath);

        return new RepositoryDiffResponse
        {
            DocumentId = document.Id,
            FromCommitId = fromCommit.Id,
            ToCommitId = toCommit.Id,
            Path = normalizedPath,
            Entries = entries
        };
    }

    private static IReadOnlyCollection<RepositoryDiffEntryResponse> BuildDiff(
        IReadOnlyCollection<TreeEntry> fromEntries,
        IReadOnlyCollection<TreeEntry> toEntries,
        string normalizedPath)
    {
        var fromMap = BuildEntryMap(fromEntries, normalizedPath);
        var toMap = BuildEntryMap(toEntries, normalizedPath);

        var paths = fromMap.Keys
            .Union(toMap.Keys, StringComparer.Ordinal)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        var result = new List<RepositoryDiffEntryResponse>(paths.Length);

        foreach (var path in paths)
        {
            fromMap.TryGetValue(path, out var oldBlobId);
            toMap.TryGetValue(path, out var newBlobId);

            if (oldBlobId is null && newBlobId is not null)
            {
                result.Add(new RepositoryDiffEntryResponse
                {
                    Path = path,
                    ChangeType = "added",
                    NewBlobId = newBlobId
                });
                continue;
            }

            if (oldBlobId is not null && newBlobId is null)
            {
                result.Add(new RepositoryDiffEntryResponse
                {
                    Path = path,
                    ChangeType = "deleted",
                    OldBlobId = oldBlobId
                });
                continue;
            }

            if (oldBlobId is not null &&
                newBlobId is not null &&
                !string.Equals(oldBlobId, newBlobId, StringComparison.Ordinal))
            {
                result.Add(new RepositoryDiffEntryResponse
                {
                    Path = path,
                    ChangeType = "modified",
                    OldBlobId = oldBlobId,
                    NewBlobId = newBlobId
                });
            }
        }

        return result;
    }

    private static Dictionary<string, string> BuildEntryMap(
        IReadOnlyCollection<TreeEntry> entries,
        string normalizedPath)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var entry in entries)
        {
            if (!IsInScope(entry.Name, normalizedPath))
            {
                continue;
            }

            result[entry.Name] = entry.ObjectId;
        }

        return result;
    }

    private static bool IsInScope(string entryPath, string normalizedPath)
    {
        if (string.IsNullOrWhiteSpace(normalizedPath))
        {
            return true;
        }

        return string.Equals(entryPath, normalizedPath, StringComparison.Ordinal) ||
               entryPath.StartsWith(normalizedPath + "/", StringComparison.Ordinal);
    }

    private static string NormalizeRepositoryPath(string? repositoryPath)
    {
        if (string.IsNullOrWhiteSpace(repositoryPath))
        {
            return string.Empty;
        }

        var normalizedPath = repositoryPath
            .Replace('\\', '/')
            .Trim()
            .Trim('/');

        foreach (var segment in normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (segment is "." or "..")
            {
                throw new ArgumentException("Repository path contains invalid segments.", nameof(repositoryPath));
            }
        }

        return normalizedPath;
    }
}
