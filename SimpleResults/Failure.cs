namespace SimpleResults;

public record Failure(string Code, FailureType FailureType, string Message, string? Description = null)
{
    public override string ToString() => Description != null ?
        $"{FailureType}:{Code}. Message: {Message}. Description: {Description}." :
        $"{FailureType}:{Code}. Message: {Message}.";

}
