using VcsDevelop.Application.Accounts.Repositories;
using VcsDevelop.Application.VcsObjects.Documents.Abstractions;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Models.Paging;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Core.Application;

namespace VcsDevelop.Application.VcsObjects.Documents.QueryHandlers;

public sealed class GetDocumentsHandler : IGetDocumentsHandler
{
    private readonly IRequestContext _requestContext;
    private readonly IAccountRepository _accountRepository;
    private readonly IDocumentRepository _documentRepository;

    public GetDocumentsHandler(
        IRequestContext requestContext,
        IAccountRepository accountRepository,
        IDocumentRepository documentRepository)
    {
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(accountRepository);
        ArgumentNullException.ThrowIfNull(documentRepository);

        _requestContext = requestContext;
        _accountRepository = accountRepository;
        _documentRepository = documentRepository;
    }

    public async Task<PagedResult<DocumentResponse>> HandleAsync(GetDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var account = await _accountRepository.GetByIdAsync(_requestContext.GetRequiredAccountId(), cancellationToken)
            .ConfigureAwait(false);

        var documents = await _documentRepository.GetAllByOwnerIdAsync(
                account.Id,
                request.PageNumber,
                request.PageSize,
                cancellationToken)
            .ConfigureAwait(false);

        var totalCount = await _documentRepository.GetCountByOwnerIdAsync(
                account.Id,
                cancellationToken)
            .ConfigureAwait(false);

        var documentsResponse = documents.Select(document => new DocumentResponse
            {
                Id = document.Id,
                Name = document.Name,
                DefaultBranchName = document.DefaultBranchName,
                Title = document.Metadata.Title,
                Description = document.Metadata.Description,
                Tags = []
            })
            .ToArray();

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedResult = new PagedResult<DocumentResponse>
        {
            Data = documentsResponse,
            Metadata = new PaginationMetadata
            {
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                HasNext = request.PageNumber < totalPages,
                HasPrevious = request.PageNumber > 1
            }
        };

        return pagedResult;
    }
}
