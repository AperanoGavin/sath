namespace PRS.Domain.Errors;

public interface IDomainError
{
    string Code { get; }
    string Title { get; }
    string Message { get; }
}

