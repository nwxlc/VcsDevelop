using GitDevelop.Application.GitObjects.Repositories;
using GitDevelop.Infrastructure.DbContexts;

namespace GitDevelop.Infrastructure.Repositories;

public abstract class BaseRepository : IBaseRepository
{
    private readonly GitDevelopDbContext _dbContext;

    protected BaseRepository(GitDevelopDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbContext = dbContext;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            //todo сделать нормальный обработчик ошибок
            Console.WriteLine(e);
            throw;
        }
    }
}
