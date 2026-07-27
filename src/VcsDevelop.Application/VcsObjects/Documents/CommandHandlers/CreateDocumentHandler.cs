using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects;
using VcsDevelop.Domain.VcsObjects.Commands;

namespace VcsDevelop.Application.VcsObjects.Documents.CommandHandlers;

public sealed class CreateDocumentHandler : ICreateDocumentHandler
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IRequestContext _requestContext;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDocumentHandler(
        IDocumentRepository documentRepository,
        IRequestContext requestContext,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(documentRepository);
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _documentRepository = documentRepository;
        _requestContext = requestContext;
        _unitOfWork = unitOfWork;
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

        _documentRepository.Add(document);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return document.Id;
    }
}
