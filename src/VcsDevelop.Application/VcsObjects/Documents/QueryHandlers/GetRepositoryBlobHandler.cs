using System.Text;
using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Application.VcsObjects.Services;
using VcsDevelop.Core.Application;

namespace VcsDevelop.Application.VcsObjects.Documents.QueryHandlers;

public sealed class GetRepositoryBlobHandler : IGetRepositoryBlobHandler
{
    private readonly IBlobRepository _blobRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ICommitRepository _commitRepository;
    private readonly ICompressionService _compressionService;
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileService _fileService;
    private readonly IRequestContext _requestContext;
    private readonly ITreeRepository _treeRepository;

    public GetRepositoryBlobHandler(
        IDocumentRepository documentRepository,
        IBranchRepository branchRepository,
        ICommitRepository commitRepository,
        ITreeRepository treeRepository,
        IBlobRepository blobRepository,
        IFileService fileService,
        ICompressionService compressionService,
        IRequestContext requestContext)
    {
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(branchRepository);
        ArgumentNullException.ThrowIfNull(commitRepository);
        ArgumentNullException.ThrowIfNull(treeRepository);
        ArgumentNullException.ThrowIfNull(blobRepository);
        ArgumentNullException.ThrowIfNull(fileService);
        ArgumentNullException.ThrowIfNull(compressionService);
        ArgumentNullException.ThrowIfNull(requestContext);

        _documentRepository = documentRepository;
        _branchRepository = branchRepository;
        _commitRepository = commitRepository;
        _treeRepository = treeRepository;
        _blobRepository = blobRepository;
        _fileService = fileService;
        _compressionService = compressionService;
        _requestContext = requestContext;
    }

    public async Task<RepositoryBlobResponse> HandleAsync(
        GetRepositoryBlobQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Path);

        var accountId = _requestContext.GetRequiredAccountId();
        var document = await _documentRepository
            .FindByIdAsync(request.DocumentId, accountId, cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new KeyNotFoundException($"Document '{request.DocumentId}' was not found.");
        }

        var normalizedPath = NormalizeRepositoryPath(request.Path);
        var branch = await _branchRepository
            .FindByDocumentAndNameAsync(document.Id, document.DefaultBranchName, cancellationToken)
            .ConfigureAwait(false);

        if (branch is null)
        {
            throw new KeyNotFoundException($"Branch '{document.DefaultBranchName}' was not found.");
        }

        var commit = await _commitRepository.FindByIdAsync(branch.HeadCommitId, cancellationToken)
            .ConfigureAwait(false);
        if (commit is null)
        {
            throw new KeyNotFoundException($"Commit '{branch.HeadCommitId}' was not found.");
        }

        var tree = await _treeRepository.FindByIdAsync(commit.RootTreeId, cancellationToken).ConfigureAwait(false);
        if (tree is null)
        {
            throw new KeyNotFoundException($"Tree '{commit.RootTreeId}' was not found.");
        }

        var entry = tree.Entries.FirstOrDefault(item =>
            string.Equals(item.Name, normalizedPath, StringComparison.Ordinal));
        if (entry is null)
        {
            throw new KeyNotFoundException($"File '{normalizedPath}' was not found.");
        }

        var blob = await _blobRepository.FindByIdAsync(entry.ObjectId, cancellationToken).ConfigureAwait(false);
        if (blob is null)
        {
            throw new KeyNotFoundException($"Blob '{entry.ObjectId}' was not found.");
        }

        var objectKey = BuildObjectKey(entry.ObjectId);

        await using var compressedStream =
            await _fileService.DownloadFileAsync(objectKey, cancellationToken).ConfigureAwait(false);
        await using var decompressedStream = new MemoryStream();
        await _compressionService
            .DecompressAsync(compressedStream, decompressedStream, cancellationToken)
            .ConfigureAwait(false);

        var content = Encoding.UTF8.GetString(decompressedStream.ToArray());

        return new RepositoryBlobResponse
        {
            DocumentId = document.Id,
            BranchName = branch.Name,
            Path = normalizedPath,
            BlobId = blob.Id,
            Size = blob.Size,
            Content = content
        };
    }

    private static string BuildObjectKey(string blobId)
        => $"objects/{blobId}";

    private static string NormalizeRepositoryPath(string repositoryPath)
    {
        var normalizedPath = repositoryPath
            .Replace('\\', '/')
            .Trim();

        normalizedPath = normalizedPath.Trim('/');

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
