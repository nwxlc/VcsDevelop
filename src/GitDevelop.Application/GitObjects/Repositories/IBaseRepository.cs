namespace GitDevelop.Application.GitObjects.Repositories;

public interface IBaseRepository
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
