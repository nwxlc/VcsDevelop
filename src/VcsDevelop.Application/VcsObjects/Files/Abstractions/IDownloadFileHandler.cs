using VcsDevelop.Application.VcsObjects.Files.Models;
using VcsDevelop.Application.VcsObjects.Files.Queries;
using VcsDevelop.Core.Application;

namespace VcsDevelop.Application.VcsObjects.Files.Abstractions;

public interface IDownloadFileHandler : IHandler<DownloadFileQuery, DownloadFileResponse>;
