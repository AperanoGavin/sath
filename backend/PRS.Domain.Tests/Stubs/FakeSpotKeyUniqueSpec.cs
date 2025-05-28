using PRS.Domain.Specifications;

namespace PRS.Domain.Tests.Stubs;

internal class FakeSpotKeyUniqueSpec(bool allowed) : ISpotKeyUniquenessSpec
{
    private readonly bool _allowed = allowed;

    public Task<bool> IsSatisfiedBy(string key, CancellationToken _)
      => Task.FromResult(_allowed);
}
