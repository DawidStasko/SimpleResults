namespace SimpleResults;

/// <summary>
/// Represents the result of an operation without returning value, including any errors or warnings encountered during execution.
/// </summary>
/// <remarks>A Result instance can contain multiple failures, categorized as errors or warnings. Use the provided
/// properties and methods to inspect, add, or merge failures. The static methods allow for convenient creation of
/// successful or failed results. Implicit conversions from Failure and Failure[] enable concise initialization of
/// failed results.</remarks>
public record class Result
{
    private List<Failure> _errors = [];
    private List<Failure> _warnings = [];

    /// <summary>
    /// Gets the collection of critical failures associated with the result.
    /// </summary>
    public IEnumerable<Failure> Errors => _errors;

    /// <summary>
    /// Gets the collection of non-critical failures associated with the result.
    /// </summary>
    public IEnumerable<Failure> Warnings => _warnings;

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
    /// Adds one or more failure instances to the result, categorizing them as errors or warnings based on their failure
    /// type.
    /// </summary>
    /// <remarks>Failures with a <see cref="FailureType"/> of <see cref="FailureType.Error"/> are added to the
    /// errors collection, while those with <see cref="FailureType.Warning"/> are added to the warnings collection.</remarks>
    /// <param name="failures">An array of <see cref="Failure"/> objects to add to the result.</param>
    /// <returns>The current <see cref="Result"/> instance with the specified failures added.</returns>
    public Result AddFailure(params Failure[] failures)
    {
        _errors.AddRange(failures.Where(x => x.FailureType is FailureType.Error));
        _warnings.AddRange(failures.Where(x => x.FailureType is FailureType.Warning));
        return this;
    }

    /// <summary>
    /// Merges the errors and warnings from the specified result into the current result.
    /// </summary>
    /// <remarks>This method is useful for aggregating results from multiple sources.
    /// Failures with a <see cref="FailureType"/> of <see cref="FailureType.Error"/> are added to the
    /// errors collection, while those with <see cref="FailureType.Warning"/> are added to the warnings collection.</remarks>
    /// <param name="externalResult">The result whose errors and warnings will be added to this result.</param>
    /// <returns>The current result instance with merged errors and warnings.</returns>
    public Result MergeIn(Result externalResult)
    {
        _errors.AddRange(externalResult.Errors);
        _warnings.AddRange(externalResult.Warnings);
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
        return Errors.Concat(Warnings).ToArray();
    }

    protected Result() { }

    /// <summary>
    /// Creates a new instance of the Result type that represents a successful operation.
    /// </summary>
    /// <returns>A <see cref="Result"/> instance with no error information.</returns>
    public static Result Success() => new();

    /// <summary>
    /// Creates a new instance of the <see cref="Result"/> type that represents an empty result.
    /// </summary>
    /// <returns>A <see cref="Result"/> instance with no error information.</returns>
    public static Result Empty() => new();

    /// <summary>
    /// Creates a failed result containing the specified failures.
    /// </summary>
    /// <param name="failures">An array of <see cref="Failure"/> instances that describe the reasons for the failure. Must contain at least one
    /// element.</param>
    /// <returns>A <see cref="Result"/> instance representing a failure with the provided failure details.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="failures"/> is empty.</exception>
    public static Result Fail(params Failure[] failures)
    {
        if (failures.Length == 0) throw new ArgumentException("Failure initialization requires to be initialized with failure.");
        var failedResult = new Result();
        failedResult.AddFailure(failures);
        return failedResult;
    }

    /// <summary>
    /// Converts a <see cref="Failure"/> instance to a <see cref="Result"/> representing a failed operation.
    /// </summary>
    /// <remarks>This implicit conversion allows a <see cref="Failure"/> to be used wherever a <see
    /// cref="Result"/> is expected, simplifying error handling code.</remarks>
    /// <param name="failures">The failure information to be encapsulated in the resulting <see cref="Result"/>.</param>
    public static implicit operator Result(Failure failures) => Fail(failures);

    /// <summary>
    /// Converts an array of failures to a <see cref="Result"/> representing a failed operation.
    /// </summary>
    /// <remarks>This operator enables implicit conversion from an array of failures to a <see
    /// cref="Result"/>. The resulting <see cref="Result"/> will indicate failure and contain the provided failure
    /// details.</remarks>
    /// <param name="failures">An array of <see cref="Failure"/> objects that describe the reasons for the failure. Cannot be null.</param>
    public static implicit operator Result(Failure[] failures) => Fail(failures);
}
