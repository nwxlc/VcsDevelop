using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VcsDevelop.Application.VcsObjects.Abstractions;
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
    public IActionResult GetByIdAsync(Guid id) => Ok();
}
