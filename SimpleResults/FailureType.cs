namespace SimpleResults;

/// <summary>
/// Specifies the type of failure encountered during an operation.
/// </summary>
/// <remarks>Use this enumeration to distinguish between non-critical warnings and critical errors when handling
/// operation results or logging failures. The value can be used to determine the appropriate response or escalation for
/// a given failure.</remarks>
public enum FailureType
{
    Warning,
    Error
}