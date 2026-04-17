using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects.Commands;

namespace VcsDevelop.Application.VcsObjects.Abstractions;

public interface ICreateDocumentHandler : IHandler<CreateDocumentCommand, Guid>;
