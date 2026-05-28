using System;

namespace Reminder_WPF.Utilities
{	
	// public class Result
	// {
	// 	private static readonly string EMPTY = "EMPTY";
	// 	protected Result(bool success, string error)
	// 	{
	// 		if (success && error != string.Empty)
	// 			throw new InvalidOperationException();
	// 		if (!success && error == string.Empty)
	// 			throw new InvalidOperationException();
	// 		Success = success;
	// 		Error = error;
	// 	}

	// 	public bool Success { get; }
	// 	public string Error { get; }
	// 	public bool IsFailure => !Success && Error != EMPTY;
	// 	public bool IsEmpty => !Success && Error == EMPTY;
	// 	public bool IsFailureOrEmpty => !Success;

	// 	public static Result Fail(string message)
	// 	{
	// 		return new Result(false, message);
	// 	}

	// 	public static Result<T?> Fail<T>(string message)
	// 	{
	// 		return new Result<T?>(default!, false, message);
	// 	}

	// 	public static Result Ok()
	// 	{
	// 		return new Result(true, string.Empty);
	// 	}

	// 	public static Result<T> Ok<T>(T value)
	// 	{
	// 		return new Result<T>(value, true, string.Empty);
	// 	}
		
	// 	public static Result Empty()
	// 	{
	// 		return new Result( false, EMPTY);
	// 	}

	// 	public static Result<T> Empty<T>()
	// 	{
	// 		return new Result<T>(default!, false, EMPTY);
	// 	}
	// }

	// public class Result<T>  :Result
	// {
	// 	protected internal Result(T value, bool success, string error)
	// 		: base(success, error)
	// 	{
	// 		Value = value;
	// 	}

	// 	public T Value { get; set; }
	// }

// 1. Define the covariant generic interface
public interface IResult<out T>
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    T? Value { get; }
    string? Error { get; }
}

// 2. Define the generic Result implementation
public class Result<T> : IResult<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string? Error { get; }

    internal Result(T? value, bool isSuccess, string? error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }
}

// 3. Define the static factory for warning-free instantiation
public static class Result
{
    // Success factory with inferred type
    public static Result<T> Success<T>(T value) => 
        new(value, true, null);

	public static Result<object> Success() => 
        new(null, true, null);

    // Failure factory for generic types
    public static Result<T> Failure<T>(string error) => 
        new(default, false, error);

    // Non-generic failure factory
    public static Result<object> Failure(string error) => 
        new(null, false, error);
}


	
}
