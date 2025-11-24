namespace EcoTradeAI.Application.Common.Models;

/// <summary>
/// Represents the result of an operation that can succeed or fail.
/// Avoids throwing exceptions for expected failures (e.g., validation errors).
/// </summary>
/// <remarks>
/// Why not just throw exceptions?
/// 
/// ❌ Exceptions are expensive (stack trace generation)
/// ❌ Exceptions are for UNEXPECTED errors (null reference, divide by zero)
/// ❌ "Email already exists" is EXPECTED - not exceptional
/// 
/// ✅ Result pattern makes success/failure explicit
/// ✅ Forces caller to handle both cases
/// ✅ More performant (no stack trace)
/// 
/// Usage:
///   var result = await _userService.RegisterAsync(command);
///   if (result.IsSuccess)
///       return Ok(result.Value);
///   else
///       return BadRequest(result.Errors);
/// </remarks>
public class Result
{
    /// <summary>
    /// Indicates whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the operation failed.
    /// Convenient alternative to !IsSuccess.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Collection of error messages if the operation failed.
    /// Empty if operation succeeded.
    /// </summary>
    public IEnumerable<string> Errors { get; }

    /// <summary>
    /// Protected constructor to force use of factory methods.
    /// </summary>
    protected Result(bool isSuccess, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? Array.Empty<string>();
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, Array.Empty<string>());

    /// <summary>
    /// Creates a failed result with a single error message.
    /// </summary>
    public static Result Failure(string error) => new(false, new[] { error });

    /// <summary>
    /// Creates a failed result with multiple error messages.
    /// </summary>
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
}

/// <summary>
/// Generic result that carries a value on success.
/// </summary>
/// <typeparam name="T">Type of the value returned on success</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// The value returned by the operation (only valid if IsSuccess = true).
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Private constructor - use factory methods.
    /// </summary>
    private Result(bool isSuccess, T? value, IEnumerable<string> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, Array.Empty<string>());

    /// <summary>
    /// Creates a failed result with a single error message.
    /// </summary>
    public new static Result<T> Failure(string error) => new(false, default, new[] { error });

    /// <summary>
    /// Creates a failed result with multiple error messages.
    /// </summary>
    public new static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
}