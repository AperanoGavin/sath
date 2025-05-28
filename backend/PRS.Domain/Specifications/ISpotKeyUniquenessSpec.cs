namespace PRS.Domain.Specifications;

public interface ISpotKeyUniquenessSpec
{
    Task<bool> IsSatisfiedBy(string key, CancellationToken ct = default);
}
