using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;

namespace VcsDevelop.Application.VcsObjects.Documents.QueryHandlers;

public sealed class GetDocumentByIdHandler : IGetDocumentByIdHandler
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IRequestContext _requestContext;

    public GetDocumentByIdHandler(
        IDocumentRepository documentRepository,
        IRequestContext requestContext)
    {
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(requestContext);

        _documentRepository = documentRepository;
        _requestContext = requestContext;
    }

    public async Task<DocumentResponse> HandleAsync(
        GetDocumentByIdQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var document = await _documentRepository
            .FindByIdAsync(
                request.Id,
                _requestContext.GetRequiredAccountId(),
                cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new KeyNotFoundException($"Document with id '{request.Id}' was not found.");
        }

        return new DocumentResponse
        {
            Name = document.Name,
            DefaultBranchName = document.DefaultBranchName,
            Title = document.Metadata.Title,
            Description = document.Metadata.Description,
            Tags = document.Metadata.Tags?.Select(tag => tag.Value).ToArray()
        };
    }
}
