namespace PRS.Presentation.Models;

public record CreateSpotRequest
{
    public string Key { get; init; } = string.Empty;
    public SpotCapability[] Capabilities { get; init; } = [];
}
