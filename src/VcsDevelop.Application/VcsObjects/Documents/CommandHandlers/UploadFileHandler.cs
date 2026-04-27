using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Models;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Application.VcsObjects.Services;
using VcsDevelop.Core.Application;
using Blob = VcsDevelop.Domain.VcsObjects.Blob;
using VcsDevelop.Domain.VcsObjects.Commands;

namespace VcsDevelop.Application.VcsObjects.Documents.CommandHandlers;

public sealed class UploadFileHandler : IUploadFileHandler
{
    private readonly IBlobRepository _blobRepository;
    private readonly IFileService _fileService;
    private readonly IRequestContext _requestContext;
    private readonly IUploadedFileRepository _uploadedFileRepository;
    private readonly ICompressionService _compressionService;
    private readonly IHashService _hashService;

    public UploadFileHandler(
        IBlobRepository blobRepository,
        IFileService fileService,
        IRequestContext requestContext,
        IUploadedFileRepository uploadedFileRepository,
        ICompressionService compressionService,
        IHashService hashService)
    {
        ArgumentNullException.ThrowIfNull(blobRepository);
        ArgumentNullException.ThrowIfNull(fileService);
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(uploadedFileRepository);
        ArgumentNullException.ThrowIfNull(compressionService);
        ArgumentNullException.ThrowIfNull(hashService);

        _blobRepository = blobRepository;
        _fileService = fileService;
        _requestContext = requestContext;
        _uploadedFileRepository = uploadedFileRepository;
        _compressionService = compressionService;
        _hashService = hashService;
    }

    public async Task<UploadFileResponse> HandleAsync(
        UploadFileCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.FileName);

        //todo добавить логику "транзакции", т.е. откатывать все если не прошло 
        await using var preparedUploadFile = await PrepareAsync(
                request.Stream,
                request.FileName,
                cancellationToken)
            .ConfigureAwait(false);

        var storageFileAsync = await StorageFileAsync(
                preparedUploadFile,
                cancellationToken)
            .ConfigureAwait(false);

        var reference = await AddUploadedFileReferenceAsync(
                _requestContext.GetRequiredAccountId(),
                storageFileAsync,
                preparedUploadFile,
                cancellationToken)
            .ConfigureAwait(false);

        return UploadFileResponse.Create(
            reference.UploadId,
            reference.BlobId,
            reference.FileName,
            reference.ObjectKey,
            reference.Size);
    }

    private async Task<PreparedUploadFile> PrepareAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var tempFilePath = Path.GetTempFileName();

        try
        {
            await using (var writeStream = File.Create(tempFilePath))
            {
                await stream.CopyToAsync(writeStream, cancellationToken).ConfigureAwait(false);
            }

            await using var readStream = File.OpenRead(tempFilePath);
            var blobId = await _hashService
                .ComputeSha1Async(readStream, cancellationToken)
                .ConfigureAwait(false);

            return PreparedUploadFile.Create(
                blobId,
                Path.GetFileName(fileName),
                tempFilePath,
                readStream.Length);
        }
        catch
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            throw;
        }
    }

    private async Task<StoredFileResult> StorageFileAsync(
        PreparedUploadFile file,
        CancellationToken cancellationToken)
    {
        var objectKey = BuildObjectKey(file.BlobId);
        var existingBlob = await _blobRepository
            .FindByIdAsync(file.BlobId, cancellationToken)
            .ConfigureAwait(false);

        if (existingBlob is not null)
        {
            return new StoredFileResult
            {
                BlobId = file.BlobId,
                ObjectKey = objectKey,
                BlobCreated = false,
                ObjectUploaded = false
            };
        }

        await using var sourceStream = File.OpenRead(file.TempFilePath);
        await using var compressedStream = await _compressionService
            .CompressAsync(sourceStream, cancellationToken)
            .ConfigureAwait(false);

        await _fileService
            .UploadFileAsync(compressedStream.Stream, objectKey, compressedStream.Length, cancellationToken)
            .ConfigureAwait(false);

        await _blobRepository
            .SetAsync(Blob.Create(file.BlobId, file.Size), cancellationToken)
            .ConfigureAwait(false);

        return new StoredFileResult
        {
            BlobId = file.BlobId,
            ObjectKey = objectKey,
            BlobCreated = true,
            ObjectUploaded = true
        };
    }

    private async Task<UploadedFileReference> AddUploadedFileReferenceAsync(
        Guid accountId,
        StoredFileResult storedFileResult,
        PreparedUploadFile preparedUploadFile,
        CancellationToken cancellationToken)
    {
        var reference = UploadedFileReference.Create(
            accountId,
            storedFileResult.BlobId,
            preparedUploadFile.FileName,
            storedFileResult.ObjectKey,
            preparedUploadFile.Size);

        await _uploadedFileRepository.SetAsync(reference, cancellationToken).ConfigureAwait(false);

        return reference;
    }

    private static string BuildObjectKey(string blobId) => $"objects/{blobId}";
}
