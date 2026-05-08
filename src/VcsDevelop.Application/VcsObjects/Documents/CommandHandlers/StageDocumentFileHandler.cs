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
            .FindByIdAsync(request.DocumentId, accountId, cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new NotFound().WithDetails($"Document '{request.DocumentId}' was not found.");
        }

        var upload = await _uploadedFileRepository
            .FindByIdAsync(request.UploadId, cancellationToken)
            .ConfigureAwait(false);

        if (upload is null || upload.AccountId != accountId)
        {
            throw new NotFound().WithDetails($"Uploaded file '{request.UploadId}' was not found.");
        }

        var stagedAt = DateTime.UtcNow;
        var repositoryPath = RepositoryPath.Create(request.RepositoryPath, upload.FileName);

        var stagedEntry = new StagedFileEntry
        {
            DocumentId = document.Id,
            AccountId = accountId,
            UploadId = upload.UploadId,
            BlobId = upload.BlobId,
            FileName = upload.FileName,
            RepositoryPath = repositoryPath.Value,
            ObjectKey = upload.ObjectKey,
            StagedAt = stagedAt
        };

        await _stagingAreaRepository.AddOrReplaceAsync(stagedEntry, cancellationToken).ConfigureAwait(false);

        return new StageDocumentFileResponse
        {
            DocumentId = document.Id,
            UploadId = upload.UploadId,
            BlobId = upload.BlobId,
            RepositoryPath = repositoryPath.Value,
            StagedAt = stagedAt
        };
    }
}
