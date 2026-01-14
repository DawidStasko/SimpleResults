namespace SimpleResults;

/// <summary>
/// Represents the result of an operation that produces a value of type <typeparamref name="T"/>, along with any
/// associated errors or warnings.
/// </summary>
/// <remarks>Use <see cref="ObjectResult{T}"/> to encapsulate the outcome of an operation, including its value,
/// errors, and warnings. This type provides properties to inspect the success or failure state and to access any
/// failures or warnings that occurred. It is commonly used to return both a result and diagnostic information from
/// methods, enabling callers to handle errors and warnings in a structured way.</remarks>
/// <typeparam name="T">The type of the value returned by the operation.</typeparam>
public record class ObjectResult<T>
{
    Result _result = Result.Empty();

    /// <summary>
    /// Gets or sets the value of the current instance.
    /// </summary>
    public T? Value { get; set; } = default(T);

    /// <summary>
    /// Gets the collection of errors associated with the result.
    /// </summary>
    public IEnumerable<Failure> Errors => _result.Errors;

    /// <summary>
    /// Gets the collection of warnings associated with the result.
    /// </summary>
    public IEnumerable<Failure> Warnings => _result.Warnings;

    /// <summary>
    /// Returns true when result does not contain any error (critical failures) or warning (non-critical failures).
    /// </summary>
    public bool IsSuccess => !Errors.Any() && !Warnings.Any();

    /// <summary>
    /// Returns true when result contains any error (critical failures) or warning (non-critical failures).
    /// </summary>
    public bool IsFailure => Errors.Any() || Warnings.Any();

    /// <summary>
    /// Returns true if result contains any error (critical failures).
    /// </summary>
    public bool HasErrors => Errors.Any();

    /// <summary>
    /// Returns true if result contains any warning (non-critical failures).
    /// </summary>
    public bool HasWarnings => Warnings.Any();

    /// <summary>
    /// Merges the failures from the specified external result into the current result, optionally overriding the
    /// current value.
    /// </summary>
    /// <remarks>This method is useful for aggregating results from multiple sources.
    /// Failures with a <see cref="FailureType"/> of <see cref="FailureType.Error"/> are added to the
    /// errors collection, while those with <see cref="FailureType.Warning"/> are added to the warnings collection. 
    /// The value is overridden only if <paramref name="overrideValue"/> is set to 
    /// <see langword="true"/>.</remarks>
    /// <param name="externalResult">An external <see cref="ObjectResult{T}"/> whose failures will be added to the current result. The value from
    /// this result may be used to override the current value if specified.</param>
    /// <param name="overrideValue">If <see langword="true"/>, the current value will be replaced with the value from <paramref
    /// name="externalResult"/>; otherwise, the current value remains unchanged.</param>
    /// <returns>The current <see cref="ObjectResult{T}"/> instance after merging failures and optionally updating the value.</returns>
    public ObjectResult<T> MergeIn(ObjectResult<T> externalResult, bool overrideValue = false)
    {
        if (overrideValue)
            Value = externalResult.Value;
        _result.AddFailure(externalResult.GetFailures());
        return this;
    }

    /// <summary>
    /// Adds one or more failure instances to the result, categorizing them as errors or warnings based on their failure
    /// type.
    /// </summary>
    /// <remarks>Failures with a <see cref="FailureType"/> of <see cref="FailureType.Error"/> are added to the
    /// errors collection, while those with <see cref="FailureType.Warning"/> are added to the warnings collection.</remarks>
    /// <param name="failures">An array of <see cref="Failure"/> objects to add to the result.</param>
    /// <returns>The current <see cref="Result"/> instance with the specified failures added.</returns>
    public ObjectResult<T> AddFailure(params Failure[] failures)
    {
        _result.AddFailure(failures);
        return this;
    }

    /// <summary>
    /// Gets an array containing all recorded failures, including both errors and warnings.
    /// </summary>
    /// <remarks>The returned array combines both errors and warnings, with errors appearing before warnings.
    /// The array is a snapshot and is not updated if the underlying failures change.</remarks>
    /// <returns>An array of <see cref="Failure"/> objects representing all errors and warnings. The array is empty if there are
    /// no failures.</returns>
    public Failure[] GetFailures()
    {
        return _result.GetFailures();
    }
    /// <summary>
    /// Creates an <see cref="ObjectResult{T}"/> that represents a successful result containing the specified value.
    /// </summary>
    /// <param name="value">The value to include in the successful result. Can be null if the result type allows it.</param>
    /// <returns>An ObjectResult<T> whose Value property is set to the specified value and indicates a successful outcome.</returns>
    public static ObjectResult<T> Success(T value) => new() { Value = value };

    /// <summary>
    /// Creates an <see cref="ObjectResult{T}"/> that represents an empty result (optionally) with the specified value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ObjectResult<T> Empty(T? value = default) => new() { Value = value };

    /// <summary>
    /// Creates a failed result containing the specified failures.
    /// </summary
    /// <param name="failures">An array of <see cref="Failure"/> instances that describe the reasons for the failure. Must contain at least one
    /// element.</param>
    /// <returns>A <see cref="ObjectResult{T}"/> instance representing a failure with the provided failure details.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="failures"/> is empty.</exception>
    public static ObjectResult<T> Fail(params Failure[] failures)
    {
        if (failures.Length == 0) throw new ArgumentException("Failure initialization requires to be initialized with failure.");
        ObjectResult<T> failedResult = new();
        failedResult.AddFailure(failures);
        return failedResult;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator ObjectResult<T>(T value) => Success(value);
    public static implicit operator ObjectResult<T>(Failure failures) => Fail(failures);
    public static implicit operator ObjectResult<T>(Failure[] failures) => Fail(failures);

    protected ObjectResult() { }
}