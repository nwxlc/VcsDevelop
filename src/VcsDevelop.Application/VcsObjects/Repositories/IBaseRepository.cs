namespace VcsDevelop.Application.VcsObjects.Repositories;

public interface IBaseRepository
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
