using SimpleResults;

namespace SimpleResultsTests;

public class ObjectResultTests
{
    [Fact]
    public void MergeIn_MerginTwoReslutWithoutOverrideFlag_ShouldContainFailuresFromBothButValueShouldNotBeOverriden()
    {
        var warning1 = new Failure("001", FailureType.Warning, "Warning1");
        var warning2 = new Failure("002", FailureType.Warning, "Warning2");
        var error1 = new Failure("003", FailureType.Error, "Error1");

        var res1 = ObjectResult<bool>.Fail(warning1);
        res1.Value = true;
        var res2 = ObjectResult<bool>.Fail(error1, warning2);
        res2.Value = false;

        var result = res1.MergeIn(res2);

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
        Assert.True(result.Value);
    }

    [Fact]
    public void MergeIn_MerginTwoReslutWithOverrideTrue_ShouldContainFailuresFromBothAndValueOverriden()
    {
        var warning1 = new Failure("001", FailureType.Warning, "Warning1");
        var warning2 = new Failure("002", FailureType.Warning, "Warning2");
        var error1 = new Failure("003", FailureType.Error, "Error1");

        var res1 = ObjectResult<bool>.Fail(warning1);
        res1.Value = true;
        var res2 = ObjectResult<bool>.Fail(error1, warning2);
        res2.Value = false;

        var result = res1.MergeIn(res2, true);

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
        Assert.False(result.Value);
    }
}
