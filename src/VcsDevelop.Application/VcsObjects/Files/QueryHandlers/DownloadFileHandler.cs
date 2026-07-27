using VcsDevelop.Application.VcsObjects.Files.Abstractions;
using VcsDevelop.Application.VcsObjects.Files.Models;
using VcsDevelop.Application.VcsObjects.Files.Queries;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Application.VcsObjects.Services;
using VcsDevelop.Core.Application;

namespace VcsDevelop.Application.VcsObjects.Files.QueryHandlers;

public sealed class DownloadFileHandler : IDownloadFileHandler
{
    private readonly IFileService _fileService;
    private readonly ICompressionService _compressionService;
    private readonly IDocumentRepository _documentRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ICommitRepository _commitRepository;
    private readonly ITreeRepository _treeRepository;
    private readonly IBlobRepository _blobRepository;
    private readonly IRequestContext _requestContext;

    public DownloadFileHandler(
        IFileService fileService,
        ICompressionService compressionService,
        IDocumentRepository documentRepository,
        IBranchRepository branchRepository,
        IRequestContext requestContext,
        ICommitRepository commitRepository,
        ITreeRepository treeRepository,
        IBlobRepository blobRepository)
    {
        ArgumentNullException.ThrowIfNull(fileService);
        ArgumentNullException.ThrowIfNull(compressionService);
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(branchRepository);
        ArgumentNullException.ThrowIfNull(commitRepository);
        ArgumentNullException.ThrowIfNull(treeRepository);
        ArgumentNullException.ThrowIfNull(blobRepository);

        _fileService = fileService;
        _compressionService = compressionService;
        _documentRepository = documentRepository;
        _requestContext = requestContext;
        _commitRepository = commitRepository;
        _treeRepository = treeRepository;
        _blobRepository = blobRepository;
        _branchRepository = branchRepository;
    }

    public async Task<DownloadFileResponse> HandleAsync(DownloadFileQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Path);

        var accountId = _requestContext.GetRequiredAccountId();

        var document = await _documentRepository
            .FindByIdAsync(
                request.RepoId,
                accountId,
                cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new KeyNotFoundException($"Document '{request.RepoId}' was not found.");
        }

        var normalizedPath = NormalizeRepositoryPath(request.Path);

        var branch = await _branchRepository
            .FindByDocumentAndNameAsync(document.Id, document.DefaultBranchName, cancellationToken)
            .ConfigureAwait(false);

        if (branch is null)
        {
            throw new KeyNotFoundException($"Branch '{document.DefaultBranchName}' was not found.");
        }

        var commit = await _commitRepository
            .FindByIdAsync(branch.HeadCommitId,
                cancellationToken)
            .ConfigureAwait(false);

        if (commit is null)
        {
            throw new KeyNotFoundException($"Commit '{branch.HeadCommitId}' was not found.");
        }

        var tree = await _treeRepository
            .FindByIdAsync(commit.RootTreeId, cancellationToken)
            .ConfigureAwait(false);

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

        var blob = await _blobRepository
            .FindByIdAsync(entry.ObjectId, cancellationToken)
            .ConfigureAwait(false);

        if (blob is null)
        {
            throw new KeyNotFoundException($"Blob '{entry.ObjectId}' was not found.");
        }

        var objectKey = BuildObjectKey(blob.Id);

        await using var downloadFile = await _fileService
            .DownloadFileAsync(objectKey, cancellationToken)
            .ConfigureAwait(false);

        var decompressedStream = new MemoryStream();
        await _compressionService
            .DecompressAsync(downloadFile.Stream, decompressedStream, cancellationToken)
            .ConfigureAwait(false);
        decompressedStream.Position = 0;

        return DownloadFileResponse.Create(
            decompressedStream,
            request.Path ?? Path.GetFileName(normalizedPath),
            downloadFile.ContentType);
    }

    private static string BuildObjectKey(string blobId)
        => $"objects/{blobId}";

    private static string NormalizeRepositoryPath(string repositoryPath)
    {
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
