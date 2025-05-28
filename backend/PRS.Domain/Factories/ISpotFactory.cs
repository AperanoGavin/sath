using PRS.Domain.Entities;
using PRS.Domain.Enums;

namespace PRS.Domain.Factories;

public interface ISpotFactory
{
    Task<Spot> Create(string key, ICollection<SpotCapability> capabilities);
}