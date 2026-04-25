namespace MiniBP.BPMS.Abstractions;

public sealed class WorkflowDefinition
{
    public required string Name { get; init; }
    public required IReadOnlyDictionary<string, WorkflowStepDefinition> Steps { get; init; }
    public required IReadOnlyList<WorkflowTransitionDefinition> Transitions { get; init; }
    public required WorkflowHooks Hooks { get; init; }

    public WorkflowStepDefinition GetStartStep() =>
        Steps.Values.Single(step => step.Type == WorkflowStepType.Start);

    public IReadOnlyList<WorkflowTransitionDefinition> GetTransitionsFrom(string stepName) =>
        Transitions.Where(transition => string.Equals(transition.FromStep, stepName, StringComparison.Ordinal))
            .ToArray();
}

public sealed class WorkflowStepDefinition
{
    public required string Name { get; init; }
    public required WorkflowStepType Type { get; init; }
    public string? Title { get; init; }
    public int Priority { get; init; }
    public IAssignmentRule AssignmentRule { get; init; } = AssignmentRules.Claiming();
    public Func<ExecutionContext, CancellationToken, Task>? ServiceAction { get; init; }
    public StepHooks Hooks { get; init; } = new();
}

public sealed class WorkflowTransitionDefinition
{
    public required string FromStep { get; init; }
    public required string ToStep { get; init; }
    public string? Name { get; init; }
    public bool IsManual { get; init; }
    public Func<ExecutionContext, bool>? Condition { get; init; }
}

public sealed class WorkflowHooks
{
    public Func<TransitionContext, CancellationToken, Task>? BeforeTransition { get; init; }
    public Func<TransitionContext, CancellationToken, Task>? AfterTransition { get; init; }
}

public sealed class StepHooks
{
    public Func<ExecutionContext, CancellationToken, Task>? BeforeDutyExecution { get; init; }
    public Func<ExecutionContext, CancellationToken, Task>? AfterDutyExecution { get; init; }
}
