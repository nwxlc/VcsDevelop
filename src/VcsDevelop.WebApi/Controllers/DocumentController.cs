using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Domain.VcsObjects.Commands;
using VcsDevelop.WebApi.Contracts.VcsObjects;

namespace VcsDevelop.WebApi.Controllers;

[ApiController]
[Route("api/repos")]
[Authorize]
public class DocumentController : ControllerBase
{
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
            "GetById",
            new { id = documentId },
            new { id = documentId });
    }

    [HttpGet("{id:guid}", Name = "GetById")]
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
}
