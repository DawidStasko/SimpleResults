using SimpleResults;

namespace SimpleResultsTests;

public class ResultTests
{
    [Fact]
    public void Initialization_FailResultWithoutFailures_ThrowsException()
    {
        Failure[] emptyFailureList = [];

        var ex = Assert.Throws<ArgumentException>(() => Result.Fail());

        Assert.Equal("Failure initialization requires to be initialized with failure.", ex.Message);
    }

    [Fact]
    public void Initialization_SuccessResult_ShouldBeCreatedWithoutErrors()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess, "This result should  be a success.");
        Assert.False(result.HasErrors, $"Failed result's property {nameof(result.HasErrors)} should return false.");
        Assert.False(result.HasWarnings, $"Failed result's property {nameof(result.HasWarnings)} should return false.");
        Assert.False(result.IsFailure, $"Failed result's property {nameof(result.IsFailure)} should return false.");

        Assert.Empty(result.Warnings);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Initialization_FailResultWithDifferentFailureTypes_FailuresAssignedProperly()
    {
        var error = new Failure("001", FailureType.Error, "Error");
        var warning = new Failure("002", FailureType.Warning, "Warning");

        var result = Result.Fail(error, warning);

        Assert.False(result.IsSuccess, "This result should not be a success.");
        Assert.True(result.HasErrors, $"Failed result's property {nameof(result.HasErrors)} should return true.");
        Assert.True(result.HasWarnings, $"Failed result's property {nameof(result.HasWarnings)} should return true.");
        Assert.True(result.IsFailure, $"Failed result's property {nameof(result.IsFailure)} should return true.");

        Assert.Single(result.Warnings);
        Assert.Single(result.Warnings, x => x.FailureType == FailureType.Warning);
        Assert.Single(result.Errors);
        Assert.Single(result.Errors, x => x.FailureType == FailureType.Error);
    }

    [Fact]
    public void Initialization_FailResultWithWarnings_FailuresAssignedProperly()
    {
        var warning1 = new Failure("001", FailureType.Warning, "Warning1");
        var warning2 = new Failure("002", FailureType.Warning, "Warning2");

        var result = Result.Fail(warning1, warning2);

        Assert.False(result.IsSuccess, "This result should not be a success.");
        Assert.False(result.HasErrors, $"Failed result's property {nameof(result.HasErrors)} should return false.");
        Assert.True(result.HasWarnings, $"Failed result's property {nameof(result.HasWarnings)} should return true.");
        Assert.True(result.IsFailure, $"Failed result's property {nameof(result.IsFailure)} should return true.");

        Assert.Equal(2, result.Warnings.Count());
        Assert.Equal(2, result.Warnings.Where(x => x.FailureType == FailureType.Warning).Count());
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Initialization_FailResultWithErrors_FailuresAssignedProperly()
    {
        var error1 = new Failure("001", FailureType.Error, "Error1");
        var error2 = new Failure("002", FailureType.Error, "Error2");

        var result = Result.Fail(error1, error2);

        Assert.False(result.IsSuccess, "This result should not be a success.");
        Assert.True(result.HasErrors, $"Failed result's property {nameof(result.HasErrors)} should return false.");
        Assert.False(result.HasWarnings, $"Failed result's property {nameof(result.HasWarnings)} should return false.");
        Assert.True(result.IsFailure, $"Failed result's property {nameof(result.IsFailure)} should return true.");

        Assert.Equal(2, result.Errors.Count());
        Assert.Equal(2, result.Errors.Where(x => x.FailureType == FailureType.Error).Count());
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void AddFailures_AddingMultipleFailures_FailuresAssignedProperly()
    {
        var warning1 = new Failure("001", FailureType.Warning, "Warning1");
        var error1 = new Failure("003", FailureType.Error, "Error1");

        var result = Result.Success();
        result.AddFailure(warning1, error1);

        Assert.False(result.IsSuccess, "This result should not be a success.");
        Assert.True(result.HasErrors, $"Failed result's property {nameof(result.HasErrors)} should return false.");
        Assert.True(result.HasWarnings, $"Failed result's property {nameof(result.HasWarnings)} should return true.");
        Assert.True(result.IsFailure, $"Failed result's property {nameof(result.IsFailure)} should return true.");

        Assert.Single(result.Warnings);
        Assert.Equal(warning1, result.Warnings.First());
        Assert.Single(result.Errors);
        Assert.Equal(error1, result.Errors.First());
    }

    [Fact]
    public void MergeIn_MerginTwoReslut_ShouldContainDataFromBoth()
    {
        var warning1 = new Failure("001", FailureType.Warning, "Warning1");
        var warning2 = new Failure("002", FailureType.Warning, "Warning2");
        var error1 = new Failure("003", FailureType.Error, "Error1");

        var result = Result.Fail(warning1).MergeIn(Result.Fail(error1, warning2));

        Assert.False(result.IsSuccess, "This result should not be a success.");
        Assert.True(result.HasErrors, $"Failed result's property {nameof(result.HasErrors)} should return true.");
        Assert.True(result.HasWarnings, $"Failed result's property {nameof(result.HasWarnings)} should return true.");
        Assert.True(result.IsFailure, $"Failed result's property {nameof(result.IsFailure)} should return true.");

        Assert.Equal(3, result.GetFailures().Length);
        Assert.Equal(2, result.Warnings.Count());
        Assert.Single(result.Warnings, x => x == warning1);
        Assert.Single(result.Warnings, x => x == warning2);
        Assert.Single(result.Errors);
        Assert.Single(result.Errors, x => x == error1);
    }

    [Fact]
    public void GetFailures_WhenCalled_ReturnAllErrorsAndWarningsAsArray()
    {
        var warning1 = new Failure("001", FailureType.Warning, "Warning1");
        var warning2 = new Failure("002", FailureType.Warning, "Warning2");
        var error1 = new Failure("003", FailureType.Error, "Error1");

        var failuresList = Result.Fail(warning1, error1, warning2).GetFailures();

        Assert.Equal(3, failuresList.Length);
        Assert.Single(failuresList, x => x == warning1);
        Assert.Single(failuresList, x => x == warning2);
        Assert.Single(failuresList, x => x == error1);
    }

    [Fact]
    public void ImplicitCasting_WhenCalledWithFailure_CreatesResult()
    {
        Result theFunction()
        {
            var warning1 = new Failure("001", FailureType.Warning, "Warning1");
            return warning1;
        }

        var result = theFunction();

        Assert.Single(result.GetFailures());
        Assert.Single(result.Warnings);


        Assert.False(result.IsSuccess, "This result should not be a success.");
        Assert.False(result.HasErrors, $"Failed result's property {nameof(result.HasErrors)} should return false.");
        Assert.True(result.HasWarnings, $"Failed result's property {nameof(result.HasWarnings)} should return true.");
        Assert.True(result.IsFailure, $"Failed result's property {nameof(result.IsFailure)} should return true.");
    }

    [Fact]
    public void ImplicitCasting_WhenCalledWithFailuresArray_CreatesResult()
    {
        Result theFunction()
        {
            Failure[] warnings = [new Failure("001", FailureType.Warning, "Warning1"), new Failure("002", FailureType.Warning, "Warning2")];
            return warnings;
        }

        var result = theFunction();

        Assert.Equal(2, result.GetFailures().Count());
        Assert.Equal(2, result.Warnings.Count());
        Assert.False(result.IsSuccess, "This result should not be a success.");
        Assert.False(result.HasErrors, $"Failed result's property {nameof(result.HasErrors)} should return false.");
        Assert.True(result.HasWarnings, $"Failed result's property {nameof(result.HasWarnings)} should return true.");
        Assert.True(result.IsFailure, $"Failed result's property {nameof(result.IsFailure)} should return true.");
    }
}
