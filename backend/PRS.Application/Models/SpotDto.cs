using PRS.Domain.Enums;

namespace PRS.Application.Models;

public record SpotDto
{
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public SpotCapability[] Capabilities { get; set; } = [];
}
