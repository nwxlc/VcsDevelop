using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Models;
using VcsDevelop.Domain.VcsObjects.Commands;
using VcsDevelop.WebApi.Contracts.VcsObjects;

namespace VcsDevelop.WebApi.Controllers;

[ApiController]
[Route("api/file")]
[Authorize]
public class FileController : ControllerBase
{
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

        await using var stream = request.File.OpenReadStream();

        var command = UploadFileCommand.Create(stream, request.File.FileName);
        var response = await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }
}
