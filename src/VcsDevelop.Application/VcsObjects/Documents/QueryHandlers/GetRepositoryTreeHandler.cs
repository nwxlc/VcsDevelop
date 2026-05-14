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
        var tree = await GetHeadTreeAsync(document.Id, document.DefaultBranchName, cancellationToken)
            .ConfigureAwait(false);
        var entries = tree is null
            ? []
            : BuildEntries(tree.Entries, normalizedPath);

        return new RepositoryTreeResponse
        {
            DocumentId = document.Id,
            BranchName = document.DefaultBranchName,
            Path = normalizedPath,
            Entries = entries
        };
    }

    private async Task<Tree?> GetHeadTreeAsync(
        Guid documentId,
        string branchName,
        CancellationToken cancellationToken)
    {
        var branch = await _branchRepository
            .FindByDocumentAndNameAsync(documentId, branchName, cancellationToken)
            .ConfigureAwait(false);

        if (branch is null)
        {
            return null;
        }

        var commit = await _commitRepository.FindByIdAsync(branch.HeadCommitId, cancellationToken)
            .ConfigureAwait(false);
        if (commit is null)
        {
            return null;
        }

        return await _treeRepository.FindByIdAsync(commit.RootTreeId, cancellationToken).ConfigureAwait(false);
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
