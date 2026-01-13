namespace SimpleResults;

public record class ObjectResult<T>
{
    Result _result = Result.Empty();

    public T? Value { get; set; } = default(T);

    public IEnumerable<Failure> Errors => _result.Errors;

    public IEnumerable<Failure> Warnings => _result.Warnings;

    public bool IsSuccess => !Errors.Any() && !Warnings.Any();

    public bool HasErrors => Errors.Any();

    public bool HasWarnings => Warnings.Any();

    public bool IsFailure => Errors.Any() || Warnings.Any();

    public ObjectResult<T> MergeIn(ObjectResult<T> externalResult, bool overrideValue = false)
    {
        if (overrideValue)
            Value = externalResult.Value;
        _result.AddFailure(externalResult.GetFailures());
        return this;
    }

    public ObjectResult<T> AddFailure(params Failure[] failures)
    {
        _result.AddFailure(failures);
        return this;
    }

    public Failure[] GetFailures()
    {
        return _result.GetFailures();
    }

    public static ObjectResult<T> Success(T value) => new() { Value = value };
    public static ObjectResult<T> Empty(T? value = default) => new() { Value = value };
    public static ObjectResult<T> Fail(params Failure[] failures)
    {
        if (failures.Length == 0) throw new ArgumentException("Failure initialization requires to be initialized with failure.");
        ObjectResult<T> failedResult = new();
        failedResult.AddFailure(failures);
        return failedResult;
    }

    public static implicit operator ObjectResult<T>(T value) => Success(value);
    public static implicit operator ObjectResult<T>(Failure failures) => Fail(failures);
    public static implicit operator ObjectResult<T>(Failure[] failures) => Fail(failures);

    protected ObjectResult() { }
}
