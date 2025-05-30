using Microsoft.EntityFrameworkCore;

using PRS.Domain.Entities;
using PRS.Domain.Repositories;
using PRS.Infrastructure.EF.Contexts;

namespace PRS.Infrastructure.EF.Repositories;

internal class EFUserRepository(AppDbContext ctx) : IUserReadOnlyRepository
{
    private readonly AppDbContext _ctx = ctx;

    public Task<User?> GetAsync(Guid id, CancellationToken ct = default)
        => _ctx.Users
               .Include(u => u.Role)
               .SingleOrDefaultAsync(u => u.Id == id, ct);

    public Task<ICollection<User>> GetAllAsync(CancellationToken ct = default)
        => _ctx.Users
               .Include(static u => u.Role)
               .ToListAsync(ct)
               .ContinueWith(static t => (ICollection<User>)t.Result, TaskContinuationOptions.OnlyOnRanToCompletion);
}
