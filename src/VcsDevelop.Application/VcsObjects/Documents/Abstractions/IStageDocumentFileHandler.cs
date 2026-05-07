using VcsDevelop.Application.VcsObjects.Documents.Entities.Models;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects.Commands;

namespace VcsDevelop.Application.VcsObjects.Documents.Abstractions;

public interface IStageDocumentFileHandler : IHandler<StageDocumentFileCommand, StageDocumentFileResponse>;
