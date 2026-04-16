using VcsDevelop.Application.VcsObjects.Abstractions;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects;
using VcsDevelop.Domain.VcsObjects.Commands;

namespace VcsDevelop.Application.VcsObjects.CommandHandlers;

public sealed class CreateDocumentHandler : ICreateDocumentHandler
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IRequestContext _requestContext;

    public CreateDocumentHandler(
        IDocumentRepository documentRepository,
        IRequestContext requestContext)
    {
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(requestContext);

        _documentRepository = documentRepository;
        _requestContext = requestContext;
    }

    public async Task<Guid> HandleAsync(
        CreateDocumentCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var existingDocument = await _documentRepository
            .FindByNameAndOwnerAsync(
                request.Name,
                _requestContext.GetRequiredAccountId(),
                cancellationToken)
            .ConfigureAwait(false);

        if (existingDocument != null)
        {
            throw new InvalidOperationException(
                $"Repository with name '{request.Name}' already exists for this account.");
        }

        var documentMetadata = DocumentMetadata.Create(
            request.Name, 
            request.Description ?? string.Empty,
            request.Tags);

        var document = Document.Create(
            _requestContext.GetRequiredAccountId(),
            request.Name,
            request.DefaultBranchName,
            documentMetadata);

        await _documentRepository.SetAsync(document, cancellationToken).ConfigureAwait(false);

        return document.Id;
    }
}
