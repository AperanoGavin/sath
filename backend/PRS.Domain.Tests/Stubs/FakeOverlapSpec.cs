using PRS.Domain.Entities;
using PRS.Domain.Specifications;

namespace PRS.Domain.Tests.Stubs;

internal class FakeOverlapSpec(bool allowed) : IReservationOverlapSpec
{
    private readonly bool _allowed = allowed;

    public Task<bool> IsSatisfiedBy(
            Spot s, DateTime from, DateTime to, CancellationToken _)
        => Task.FromResult(_allowed);
}
