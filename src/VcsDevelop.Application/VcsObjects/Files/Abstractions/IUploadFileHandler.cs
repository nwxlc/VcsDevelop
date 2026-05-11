using VcsDevelop.Application.VcsObjects.Files.Commands;
using VcsDevelop.Application.VcsObjects.Files.Models;
using VcsDevelop.Core.Application;

namespace VcsDevelop.Application.VcsObjects.Files.Abstractions;

public interface IUploadFileHandler : IHandler<UploadFileCommand, UploadFileResponse>;
