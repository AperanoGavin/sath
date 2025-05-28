using PRS.Domain.Entities;
using PRS.Domain.Repositories;

namespace PRS.Domain.Tests.Stubs;


internal class StubUserRepo(User? user) : IUserReadOnlyRepository
{
    private readonly User? _user = user;

    public Task<User?> GetAsync(Guid id, CancellationToken _)
        => Task.FromResult(_user);

    public Task<ICollection<User>> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();
}
