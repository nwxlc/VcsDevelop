using VcsDevelop.Core.Application;
using VcsDevelop.Infrastructure.DbContexts;

namespace VcsDevelop.Infrastructure.Repositories.VcsObjects;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly VcsDevelopDbContext _context;

    public UnitOfWork(VcsDevelopDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
