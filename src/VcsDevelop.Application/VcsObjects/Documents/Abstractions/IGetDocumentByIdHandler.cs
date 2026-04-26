using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Application.VcsObjects.Documents.Entities.Queries;
using VcsDevelop.Core.Application;

namespace VcsDevelop.Application.VcsObjects.Documents.Abstractions;

public interface IGetDocumentByIdHandler : IHandler<GetDocumentByIdQuery, DocumentResponse>;
