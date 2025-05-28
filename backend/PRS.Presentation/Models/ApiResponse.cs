namespace PRS.Presentation.Models;

// TODO: Add support for pagination
public record ApiResponse<T>
{
    public required T Data { get; set; }
    public object? Meta { get; set; }
}
