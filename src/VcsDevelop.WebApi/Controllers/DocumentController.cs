using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Application.VcsObjects.Files.Abstractions;
using VcsDevelop.Application.VcsObjects.Files.Commands;
using VcsDevelop.Application.VcsObjects.Files.Models;
using VcsDevelop.Domain.VcsObjects.Commands;
using VcsDevelop.WebApi.Contracts.VcsObjects;

namespace VcsDevelop.WebApi.Controllers;

[ApiController]
[Route("api/repos")]
[Authorize]
public class DocumentController : ControllerBase
{
    private const int MaxFileNameLength = 255;

    [HttpPost("create")]
    public async Task<IActionResult> CreateDocumentAsync(
        [FromBody]CreateDocumentRequest request,
        [FromServices]ICreateDocumentHandler handler,
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

        return CreatedAtAction(
            "GetDocumentById",
            new { id = documentId },
            new { id = documentId });
    }

    [HttpGet("{id:guid}", Name = "GetById")]
    [HttpGet("{id:guid}", Name = "GetDocumentById")]
    public async Task<ActionResult<DocumentResponse>> GetByIdAsync(
        Guid id,
        [FromServices]IGetDocumentByIdHandler handler,
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
        [FromForm]UploadFileRequest request,
        [FromServices]IUploadFileHandler handler,
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

        await using var stream = request.File.OpenReadStream();

        var command = UploadFileCommand.Create(id, stream, normalizedFileName);
        var response = await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        return Ok(response);
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
