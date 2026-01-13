namespace SimpleResults;

public record class Result
{
    private List<Failure> _errors = [];
    private List<Failure> _warnings = [];

    public IEnumerable<Failure> Errors => _errors;

    public IEnumerable<Failure> Warnings => _warnings;

    public bool IsSuccess => !Errors.Any() && !Warnings.Any();

    public bool HasErrors => Errors.Any();

    public bool HasWarnings => Warnings.Any();

    public bool IsFailure => Errors.Any() || Warnings.Any();

    public Result AddFailure(params Failure[] failures)
    {
        _errors.AddRange(failures.Where(x => x.FailureType is FailureType.Error));
        _warnings.AddRange(failures.Where(x => x.FailureType is FailureType.Warning));
        return this;
    }

    public Result MergeIn(Result externalValidation)
    {
        _errors.AddRange(externalValidation.Errors);
        _warnings.AddRange(externalValidation.Warnings);
        return this;
    }

    public Failure[] GetFailures()
    {
        return Errors.Concat(Warnings).ToArray();
    }

    protected Result() { }

    public static Result Success() => new();

    public static Result Empty() => new();

    public static Result Fail(params Failure[] failures)
    {
        if (failures.Length == 0) throw new ArgumentException("Failure initialization requires to be initialized with failure.");
        var failedResult = new Result();
        failedResult.AddFailure(failures);
        return failedResult;
    }

    public static implicit operator Result(Failure failures) => Fail(failures);
    public static implicit operator Result(Failure[] failures) => Fail(failures);
}
