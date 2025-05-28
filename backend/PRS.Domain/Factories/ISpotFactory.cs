using PRS.Domain.Core;
using PRS.Domain.Entities;
using PRS.Domain.Enums;

namespace PRS.Domain.Factories;

public interface ISpotFactory
{
    Task<Result<Spot>> Create(string key, ICollection<SpotCapability> capabilities);
}