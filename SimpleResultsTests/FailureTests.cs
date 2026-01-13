using SimpleResults;

namespace SimpleResultsTests;

public class FailureTests
{
    [Fact]
    public void ToString_WhenNoDescription_ReturnsStringWithoutDescription()
    {
        var failure = new Failure("001", FailureType.Warning, "Title");

        var failureString = failure.ToString();

        Assert.Equal("Warning:001. Message: Title.", failureString);
    }

    [Fact]
    public void ToString_WhenDescriptionNotNull_ReturnsStringWithDescription()
    {
        var failure = new Failure("001", FailureType.Warning, "Title", "Long description");

        var failureString = failure.ToString();

        Assert.Equal("Warning:001. Message: Title. Description: Long description.", failureString);
    }
}
