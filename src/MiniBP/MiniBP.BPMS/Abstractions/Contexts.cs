namespace MiniBP.BPMS.Abstractions;

public sealed class ExecutionContext
{
    public required CaseInstance Case { get; init; }
    public required WorkflowDefinition Workflow { get; init; }
    public required IServiceProvider Services { get; init; }
    public required string CurrentStep { get; init; }
    public string? ActingUserId { get; init; }
    public Dictionary<string, object?> FlowParameters => Case.FlowParameters;

    public T? GetParameter<T>(string key)
    {
        if (!FlowParameters.TryGetValue(key, out var value) || value is null) {
            return default;
        }

        if (value is T typedValue) {
            return typedValue;
        }

        return (T?)Convert.ChangeType(value, typeof(T));
    }
}

public sealed class TransitionContext
{
    public required CaseInstance Case { get; init; }
    public required WorkflowDefinition Workflow { get; init; }
    public required string FromStep { get; init; }
    public required string ToStep { get; init; }
    public string? ActingUserId { get; init; }
}

public sealed class AssignmentContext
{
    public required CaseInstance Case { get; init; }
    public required WorkflowStepDefinition Step { get; init; }
    public required IServiceProvider Services { get; init; }
    public string? ActingUserId { get; init; }
}
