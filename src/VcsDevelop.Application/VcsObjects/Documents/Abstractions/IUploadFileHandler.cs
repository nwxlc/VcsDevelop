using VcsDevelop.Application.VcsObjects.Models;
using VcsDevelop.Core.Application;
using VcsDevelop.Domain.VcsObjects.Commands;

namespace VcsDevelop.Application.VcsObjects.Documents.Abstractions;

public interface IUploadFileHandler : IHandler<UploadFileCommand, UploadFileResponse>;
