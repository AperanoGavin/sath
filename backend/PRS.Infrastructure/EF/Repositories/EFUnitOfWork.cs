using PRS.Domain.Repositories;
using PRS.Infrastructure.EF.Contexts;

namespace PRS.Infrastructure.EF.Repositories;

internal class EFUnitOfWork(AppDbContext ctx) : IUnitOfWork
{
    private readonly AppDbContext _ctx = ctx;

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        return _ctx.SaveChangesAsync(cancellationToken);
    }
}
