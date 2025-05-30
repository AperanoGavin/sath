namespace PRS.Domain.Errors;

/// <summary>
/// A catch-all for anything unexpected.
/// </summary>
public sealed record UnknownError(string Detail) : IDomainError
{
    public string Code => "UnhandledError";
    public string Title => "An unexpected error occurred";
    public string Message => Detail;
}
