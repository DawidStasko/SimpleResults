namespace SimpleResults;

/// <summary>
/// Represents a standardized failure result, including a code, type, message, and optional description.
/// </summary>
/// <remarks>Use this record to convey error or failure information in a consistent manner across application
/// boundaries. The properties provide structured data for programmatic handling as well as user-facing error
/// messages.</remarks>
/// <param name="Code">The unique code that identifies the specific failure condition.</param>
/// <param name="FailureType">The category or type of failure represented by this instance.</param>
/// <param name="Message">A human-readable message describing the failure.</param>
/// <param name="Description">An optional detailed description providing additional context about the failure. May be null if no further details
/// are available.</param>
public record Failure(string Code, FailureType FailureType, string Message, string? Description = null)
{
    public override string ToString() => Description != null ?
        $"{FailureType}:{Code}. Message: {Message}. Description: {Description}." :
        $"{FailureType}:{Code}. Message: {Message}.";

}
