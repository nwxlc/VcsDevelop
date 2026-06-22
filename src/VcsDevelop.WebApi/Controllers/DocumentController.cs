using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Application.VcsObjects.Files.Abstractions;
using VcsDevelop.Application.VcsObjects.Files.Commands;
using VcsDevelop.Application.VcsObjects.Files.Models;
using VcsDevelop.Application.VcsObjects.Files.Queries;
using VcsDevelop.Domain.VcsObjects.Commands;
using VcsDevelop.WebApi.Contracts.VcsObjects;

namespace VcsDevelop.WebApi.Controllers;

[ApiController]
[Route("api/repos")]
[Authorize]
public class DocumentController : ControllerBase
{
    private const int MaxFileNameLength = 255;
    private const int MaxPageSize = 100;
    private const string DefaultContentType = "application/octet-stream";

    [HttpPost("create")]
    public async Task<IActionResult> CreateDocumentAsync(
        [FromBody] CreateDocumentRequest request,
        [FromServices] ICreateDocumentHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(handler);

        var command = CreateDocumentCommand.Create(
            request.Name,
            request.DefaultBranchName ?? "main",
            request.Description,
            request.Tags);

        var documentId = await handler.HandleAsync(command, cancellationToken)
            .ConfigureAwait(false);

        return CreatedAtRoute(
            "GetDocumentById",
            new { id = documentId },
            new { id = documentId });
    }

    [HttpGet]
    public async Task<IActionResult> GetDocumentsAsync(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromServices] IGetDocumentsHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (page < 1)
        {
            return BadRequest("Page must be greater than or equal to 1.");
        }

        if (pageSize is < 1 or > MaxPageSize)
        {
            return BadRequest($"Page size must be between 1 and {MaxPageSize}.");
        }

        var query = GetDocumentsQuery.Create(page, pageSize);

        var response = await handler
            .HandleAsync(query, cancellationToken)
            .ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("{id:guid}", Name = "GetDocumentById")]
    public async Task<ActionResult<DocumentResponse>> GetByIdAsync(
        Guid id,
        [FromServices] IGetDocumentByIdHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var query = GetDocumentByIdQuery.Create(id);

        try
        {
            var response = await handler.HandleAsync(query, cancellationToken).ConfigureAwait(false);

            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/upload")]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = 52428800)]
    [RequestSizeLimit(52428800)]
    public async Task<ActionResult<UploadFileResponse>> UploadFileAsync(
        Guid id,
        [FromForm] UploadFileRequest request,
        [FromServices] IUploadFileHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request.File);

        if (request.File.Length == 0)
        {
            return BadRequest("File is empty.");
        }

        var normalizedFileName = ValidateFileName(request.File.FileName);
        if (normalizedFileName is null)
        {
            return BadRequest("File name is invalid.");
        }

        var contentType = string.IsNullOrWhiteSpace(request.File.ContentType)
            ? DefaultContentType
            : request.File.ContentType;

        await using var stream = request.File.OpenReadStream();

        var command = UploadFileCommand.Create(id, stream, normalizedFileName, contentType);
        var response = await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> DownloadFileAsync(
        Guid id,
        [FromQuery] string path,
        [FromServices] IDownloadFileHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handler);

        try
        {
            var query = DownloadFileQuery.Create(id, path);
            var response = await handler
                .HandleAsync(query, cancellationToken)
                .ConfigureAwait(false);

            return File(response.Stream, response.ContentType, response.FileName);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/stage")]
    public async Task<ActionResult<StageDocumentFileResponse>> StageFileAsync(
        Guid id,
        [FromBody] StageDocumentFileRequest request,
        [FromServices] IStageDocumentFileHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(handler);

        var command = StageDocumentFileCommand.Create(
            id,
            request.UploadId,
            request.RepositoryPath ?? string.Empty);

        var response = await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPost("{id:guid}/commit")]
    public async Task<ActionResult<CommitDocumentResponse>> CommitAsync(
        Guid id,
        [FromBody] CommitDocumentRequest request,
        [FromServices] ICommitDocumentHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(handler);

        var command = CommitDocumentCommand.Create(id, request.Message);
        var response = await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("{id:guid}/tree")]
    public async Task<ActionResult<RepositoryTreeResponse>> GetRepositoryTreeAsync(
        Guid id,
        [FromQuery] string? path,
        [FromServices] IGetRepositoryTreeHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handler);

        try
        {
            var query = GetRepositoryTreeQuery.Create(id, path);
            var response = await handler.HandleAsync(query, cancellationToken)
                .ConfigureAwait(false);

            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{id:guid}/blob")]
    public async Task<ActionResult<RepositoryBlobResponse>> GetBlobAsync(
        Guid id,
        [FromQuery] string? path,
        [FromServices] IGetRepositoryBlobHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (string.IsNullOrWhiteSpace(path))
        {
            return BadRequest("Path is required.");
        }

        try
        {
            var getRepositoryBlobQuery = GetRepositoryBlobQuery.Create(id, path);
            var response = await handler.HandleAsync(getRepositoryBlobQuery, cancellationToken)
                .ConfigureAwait(false);

            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{id:guid}/log")]
    public async Task<ActionResult<DocumentLogResponse>> GetLogAsync(
        Guid id,
        [FromServices] IGetDocumentLogHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var query = GetDocumentLogQuery.Create(id);

        var response = await handler.HandleAsync(query, cancellationToken)
            .ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPost("{id:guid}/revert")]
    public async Task<ActionResult<RevertDocumentResponse>> RevertAsync(
        Guid id,
        [FromBody] RevertDocumentRequest request,
        [FromServices] IRevertDocumentHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);

        var command = RevertDocumentCommand.Create(id, request.CommitId, request.BranchName);
        var response = await handler.HandleAsync(command, cancellationToken)
            .ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("{id:guid}/diff")]
    public async Task<ActionResult<RepositoryDiffResponse>> GetDiffAsync(
        Guid id,
        [FromQuery] string fromCommitId,
        [FromQuery] string toCommitId,
        [FromQuery] string? path,
        [FromServices] IGetRepositoryDiffHandler handler,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handler);

        try
        {
            var query = GetRepositoryDiffQuery.Create(id, fromCommitId, toCommitId, path);
            var response = await handler.HandleAsync(query, cancellationToken)
                .ConfigureAwait(false);

            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    private static string? ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        var normalizedFileName = Path.GetFileName(fileName.Trim());
        if (string.IsNullOrWhiteSpace(normalizedFileName) || normalizedFileName.Length > MaxFileNameLength)
        {
            return null;
        }

        return normalizedFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            ? null
            : normalizedFileName;
    }
}
