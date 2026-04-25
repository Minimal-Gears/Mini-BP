namespace MiniBP.BPMS.Abstractions;

public sealed class StartCaseRequest
{
    public required string WorkflowName { get; init; }
    public string? StartedByUserId { get; init; }
    public IDictionary<string, object?> Parameters { get; init; } = new Dictionary<string, object?>();
}

public sealed class CompleteDutyRequest
{
    public required string CompletedByUserId { get; init; }
    public string? TransitionName { get; init; }
    public string? Comment { get; init; }
    public IDictionary<string, object?> Parameters { get; init; } = new Dictionary<string, object?>();
}
