using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Core.Errors;
using VcsDevelop.Domain.VcsObjects.Commands;

namespace VcsDevelop.Application.VcsObjects.Documents.CommandHandlers;

public sealed class StageDocumentFileHandler : IStageDocumentFileHandler
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IRequestContext _requestContext;
    private readonly IStagingAreaRepository _stagingAreaRepository;
    private readonly IUploadedFileRepository _uploadedFileRepository;

    public StageDocumentFileHandler(
        IDocumentRepository documentRepository,
        IRequestContext requestContext,
        IUploadedFileRepository uploadedFileRepository,
        IStagingAreaRepository stagingAreaRepository)
    {
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(uploadedFileRepository);
        ArgumentNullException.ThrowIfNull(stagingAreaRepository);

        _documentRepository = documentRepository;
        _requestContext = requestContext;
        _uploadedFileRepository = uploadedFileRepository;
        _stagingAreaRepository = stagingAreaRepository;
    }

    public async Task<StageDocumentFileResponse> HandleAsync(
        StageDocumentFileCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var accountId = _requestContext.GetRequiredAccountId();
        
        var document = await _documentRepository
            .GetByIdAsync(request.DocumentId, cancellationToken)
            .ConfigureAwait(false);
        
        var upload = await _uploadedFileRepository
            .FindByIdAsync(request.UploadId, cancellationToken)
            .ConfigureAwait(false);
        
        if (upload is null || upload.AccountId != accountId)
        {
            throw new NotFound().WithDetails($"Uploaded file '{request.UploadId}' was not found.");
        }

        var stagedAt = DateTime.UtcNow;
        var repositoryPath = NormalizeRepositoryPath(request.RepositoryPath);

        var stagedEntry = new StagedFileEntry
        {
            DocumentId = document.Id,
            AccountId = accountId,
            UploadId = upload.UploadId,
            BlobId = upload.BlobId,
            FileName = upload.FileName,
            RepositoryPath = repositoryPath,
            ObjectKey = upload.ObjectKey,
            StagedAt = stagedAt
        };
        
        await _stagingAreaRepository.AddOrReplaceAsync(stagedEntry, cancellationToken).ConfigureAwait(false);

        return new StageDocumentFileResponse
        {
            DocumentId = document.Id,
            UploadId = upload.UploadId,
            BlobId = upload.BlobId,
            RepositoryPath = repositoryPath,
            StagedAt = stagedAt
        };
    }

    private static string NormalizeRepositoryPath(string? repositoryPath)
    {
        if (string.IsNullOrWhiteSpace(repositoryPath))
        {
            return string.Empty;
        }

        return repositoryPath
            .Replace('\\', '/')
            .TrimStart('/');
    }
}
