namespace PRS.Domain.Errors;

public record DomainError(string Code, string Title, string Message) : IDomainError;

