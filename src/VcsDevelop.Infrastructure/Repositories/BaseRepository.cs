using VcsDevelop.Application.VcsObjects.Repositories;
using VcsDevelop.Infrastructure.DbContexts;

namespace VcsDevelop.Infrastructure.Repositories;

public abstract class BaseRepository : IBaseRepository
{
    private readonly VcsDevelopDbContext _dbContext;

    protected BaseRepository(VcsDevelopDbContext dbContext)
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
