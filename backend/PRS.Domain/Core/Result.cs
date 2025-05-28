using System.Diagnostics.CodeAnalysis;

using PRS.Domain.Errors;

namespace PRS.Domain.Core;

public sealed class Result<T>
{
    private readonly T? _value;

    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    [MemberNotNullWhen(false, nameof(_value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    public IDomainError? Error { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    private Result(bool isSuccess, T? value, IDomainError? error)
    {
        IsSuccess = isSuccess;
        _value = value;
        Error = error;
    }

    public static Result<T> Success(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "Success value cannot be null.");
        }

        return new Result<T>(true, value, null);
    }

    public static Result<T> Failure(IDomainError error)
    {
        if (error is null)
        {
            throw new ArgumentException("Error message cannot be null or empty.", nameof(error));
        }

        return new Result<T>(false, default, error);
    }
}

public sealed class Result
{
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    public IDomainError? Error { get; }

    private Result(bool isSuccess, IDomainError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(IDomainError error)
    {
        if (error is null)
        {
            throw new ArgumentException("Error cannot be null or whitespace.", nameof(error));
        }

        return new Result(false, error);
    }
}
