using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Files.Abstractions;
using VcsDevelop.Application.VcsObjects.Files.Commands;
using VcsDevelop.Application.VcsObjects.Files.Models;
using VcsDevelop.WebApi.Contracts.VcsObjects;

namespace VcsDevelop.WebApi.Controllers;

[ApiController]
[Route("api/file")]
[Authorize]
public class FileController : ControllerBase
{
    private const int MaxFileNameLength = 255;

    [HttpPost("files/upload")]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(MultipartBodyLengthLimit = 52428800)]
    [RequestSizeLimit(52428800)]
    public async Task<ActionResult<UploadFileResponse>> UploadFileAsync(
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

        var command = UploadFileCommand.Create(stream, normalizedFileName);
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
