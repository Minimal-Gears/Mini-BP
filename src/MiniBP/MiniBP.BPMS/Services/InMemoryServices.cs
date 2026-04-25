namespace MiniBP.BPMS.Abstractions;

public sealed class InMemoryWorkflowRegistry : IWorkflowRegistry
{
    private readonly Dictionary<string, WorkflowDefinition> _workflows = new(StringComparer.Ordinal);

    public void Register(WorkflowDefinition workflow) => _workflows[workflow.Name] = workflow;

    public WorkflowDefinition Get(string workflowName) =>
        _workflows.TryGetValue(workflowName, out var workflow)
            ? workflow
            : throw new InvalidOperationException($"Workflow '{workflowName}' is not registered.");

    public IReadOnlyCollection<WorkflowDefinition> GetAll() => _workflows.Values.ToArray();
}

public sealed class InMemoryRuntimeStore
{
    public Dictionary<Guid, CaseInstance> Cases { get; } = [];
    public Dictionary<Guid, Duty> Duties { get; } = [];
}

public sealed class CaseService(
    IWorkflowRegistry workflowRegistry,
    InMemoryRuntimeStore store,
    IServiceProvider serviceProvider) : ICaseService
{
    private readonly WorkflowRuntime _runtime = new(workflowRegistry, store, serviceProvider);

    public async Task<CaseInstance> StartCaseAsync(StartCaseRequest request, CancellationToken cancellationToken = default)
    {
        var workflow = workflowRegistry.Get(request.WorkflowName);
        var startStep = workflow.GetStartStep();

        var instance = new CaseInstance {
            WorkflowName = workflow.Name,
            CurrentStep = startStep.Name,
            Status = CaseStatus.Draft
        };

        foreach (var pair in request.Parameters) {
            instance.FlowParameters[pair.Key] = pair.Value;
        }

        instance.Tracks.Add(new TrackEntry {
            Type = TrackType.CaseStarted,
            Message = $"Case started in workflow '{workflow.Name}'.",
            Data = new Dictionary<string, object?> {
                ["workflow"] = workflow.Name,
                ["startedBy"] = request.StartedByUserId
            }
        });

        store.Cases[instance.Id] = instance;
        await _runtime.AdvanceUntilWaitStateAsync(instance, request.StartedByUserId, cancellationToken);
        return instance;
    }

    public Task<CaseInstance?> GetCaseAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        store.Cases.TryGetValue(caseId, out var instance);
        return Task.FromResult(instance);
    }

    public Task<IReadOnlyCollection<CaseInstance>> GetCasesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<CaseInstance>>(store.Cases.Values.OrderByDescending(item => item.CreatedAtUtc).ToArray());
}

public sealed class DutyService(
    IWorkflowRegistry workflowRegistry,
    InMemoryRuntimeStore store,
    IServiceProvider serviceProvider) : IDutyService
{
    private readonly WorkflowRuntime _runtime = new(workflowRegistry, store, serviceProvider);

    public Task<IReadOnlyCollection<Duty>> GetInboxAsync(string? userId = null, CancellationToken cancellationToken = default)
    {
        var query = store.Duties.Values.Where(duty => !duty.IsCompleted);

        if (!string.IsNullOrWhiteSpace(userId)) {
            query = query.Where(duty => duty.Assignee is null || string.Equals(duty.Assignee, userId, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult<IReadOnlyCollection<Duty>>(query
            .OrderByDescending(duty => duty.Priority)
            .ThenBy(duty => duty.CreatedAtUtc)
            .ToArray());
    }

    public Task<Duty?> GetDutyAsync(Guid dutyId, CancellationToken cancellationToken = default)
    {
        store.Duties.TryGetValue(dutyId, out var duty);
        return Task.FromResult(duty);
    }

    public Task<Duty> ClaimDutyAsync(Guid dutyId, string userId, CancellationToken cancellationToken = default)
    {
        var duty = GetOpenDuty(dutyId);

        if (!string.IsNullOrWhiteSpace(duty.Assignee) && !string.Equals(duty.Assignee, userId, StringComparison.OrdinalIgnoreCase)) {
            throw new InvalidOperationException($"Duty '{dutyId}' is already assigned to '{duty.Assignee}'.");
        }

        duty.Assignee = userId;
        AddTrack(duty.CaseId, TrackType.DutyClaimed, $"Duty '{duty.Title}' claimed by '{userId}'.", new Dictionary<string, object?> {
            ["dutyId"] = duty.Id,
            ["userId"] = userId
        });
        return Task.FromResult(duty);
    }

    public async Task<Duty> CompleteDutyAsync(Guid dutyId, CompleteDutyRequest request, CancellationToken cancellationToken = default)
    {
        var duty = GetOpenDuty(dutyId);

        if (!string.IsNullOrWhiteSpace(duty.Assignee) && !string.Equals(duty.Assignee, request.CompletedByUserId, StringComparison.OrdinalIgnoreCase)) {
            throw new InvalidOperationException($"Duty '{dutyId}' is assigned to '{duty.Assignee}', not '{request.CompletedByUserId}'.");
        }

        if (duty.Assignee is null) {
            duty.Assignee = request.CompletedByUserId;
        }

        if (request.Comment is { Length: > 0 }) {
            await AddCommentAsync(dutyId, request.CompletedByUserId, request.Comment, cancellationToken);
        }

        var instance = store.Cases[duty.CaseId];
        foreach (var pair in request.Parameters) {
            instance.FlowParameters[pair.Key] = pair.Value;
            AddTrack(instance.Id, TrackType.ParameterChanged, $"Parameter '{pair.Key}' changed.", new Dictionary<string, object?> {
                ["name"] = pair.Key,
                ["value"] = pair.Value
            });
        }

        duty.CompletedAtUtc = DateTimeOffset.UtcNow;
        AddTrack(instance.Id, TrackType.DutyCompleted, $"Duty '{duty.Title}' completed by '{request.CompletedByUserId}'.", new Dictionary<string, object?> {
            ["dutyId"] = duty.Id,
            ["userId"] = request.CompletedByUserId
        });

        await _runtime.MovePastUserStepAsync(instance, duty, request, cancellationToken);
        return duty;
    }

    public Task<DutyComment> AddCommentAsync(Guid dutyId, string userId, string text, CancellationToken cancellationToken = default)
    {
        var duty = GetOpenOrClosedDuty(dutyId);
        var comment = new DutyComment {
            AuthorUserId = userId,
            Text = text
        };

        duty.Comments.Add(comment);
        AddTrack(duty.CaseId, TrackType.CommentAdded, $"Comment added by '{userId}'.", new Dictionary<string, object?> {
            ["dutyId"] = duty.Id,
            ["userId"] = userId
        });
        return Task.FromResult(comment);
    }

    private Duty GetOpenDuty(Guid dutyId)
    {
        var duty = GetOpenOrClosedDuty(dutyId);
        if (duty.IsCompleted) {
            throw new InvalidOperationException($"Duty '{dutyId}' has already been completed.");
        }

        return duty;
    }

    private Duty GetOpenOrClosedDuty(Guid dutyId) =>
        store.Duties.TryGetValue(dutyId, out var duty)
            ? duty
            : throw new InvalidOperationException($"Duty '{dutyId}' was not found.");

    private void AddTrack(Guid caseId, TrackType type, string message, Dictionary<string, object?> data)
    {
        store.Cases[caseId].Tracks.Add(new TrackEntry {
            Type = type,
            Message = message,
            Data = data
        });
    }
}

internal sealed class WorkflowRuntime(
    IWorkflowRegistry workflowRegistry,
    InMemoryRuntimeStore store,
    IServiceProvider serviceProvider)
{
    public async Task AdvanceUntilWaitStateAsync(CaseInstance instance, string? actingUserId, CancellationToken cancellationToken)
    {
        while (instance.Status is not CaseStatus.Done and not CaseStatus.Cancelled and not CaseStatus.Paused) {
            var workflow = workflowRegistry.Get(instance.WorkflowName);
            var step = workflow.Steps[instance.CurrentStep];
            var context = new ExecutionContext {
                Case = instance,
                Workflow = workflow,
                Services = serviceProvider,
                CurrentStep = step.Name,
                ActingUserId = actingUserId
            };

            switch (step.Type) {
                case WorkflowStepType.Start:
                    await TransitionToNextAsync(instance, workflow, step, context, null, actingUserId, cancellationToken);
                    break;
                case WorkflowStepType.Service:
                    if (step.ServiceAction is not null) {
                        await step.ServiceAction(context, cancellationToken);
                    }

                    await TransitionToNextAsync(instance, workflow, step, context, null, actingUserId, cancellationToken);
                    break;
                case WorkflowStepType.Gateway:
                    await TransitionToNextAsync(instance, workflow, step, context, null, actingUserId, cancellationToken);
                    break;
                case WorkflowStepType.End:
                    instance.Status = CaseStatus.Done;
                    instance.CompletedAtUtc = DateTimeOffset.UtcNow;
                    instance.Tracks.Add(new TrackEntry {
                        Type = TrackType.CaseCompleted,
                        Message = $"Case completed at step '{step.Name}'."
                    });
                    return;
                case WorkflowStepType.User:
                    EnsureDutyExists(instance, workflow, step, actingUserId);
                    instance.Status = CaseStatus.ToDo;
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public async Task MovePastUserStepAsync(CaseInstance instance, Duty duty, CompleteDutyRequest request, CancellationToken cancellationToken)
    {
        var workflow = workflowRegistry.Get(instance.WorkflowName);
        var step = workflow.Steps[instance.CurrentStep];
        var context = new ExecutionContext {
            Case = instance,
            Workflow = workflow,
            Services = serviceProvider,
            CurrentStep = step.Name,
            ActingUserId = request.CompletedByUserId
        };

        if (step.Hooks.BeforeDutyExecution is not null) {
            await step.Hooks.BeforeDutyExecution(context, cancellationToken);
        }

        if (step.Hooks.AfterDutyExecution is not null) {
            await step.Hooks.AfterDutyExecution(context, cancellationToken);
        }

        await TransitionToNextAsync(instance, workflow, step, context, request.TransitionName, request.CompletedByUserId, cancellationToken);
        await AdvanceUntilWaitStateAsync(instance, request.CompletedByUserId, cancellationToken);
    }

    private void EnsureDutyExists(CaseInstance instance, WorkflowDefinition workflow, WorkflowStepDefinition step, string? actingUserId)
    {
        var openDutyExists = store.Duties.Values.Any(duty =>
            duty.CaseId == instance.Id &&
            string.Equals(duty.StepName, step.Name, StringComparison.Ordinal) &&
            !duty.IsCompleted);

        if (openDutyExists) {
            return;
        }

        var assignee = step.AssignmentRule.ResolveAssignee(new AssignmentContext {
            Case = instance,
            Step = step,
            Services = serviceProvider,
            ActingUserId = actingUserId
        });

        var duty = new Duty {
            CaseId = instance.Id,
            StepName = step.Name,
            Title = step.Title ?? step.Name,
            Assignee = assignee,
            Priority = step.Priority
        };

        store.Duties[duty.Id] = duty;
        instance.Tracks.Add(new TrackEntry {
            Type = TrackType.DutyCreated,
            Message = $"Duty '{duty.Title}' created.",
            Data = new Dictionary<string, object?> {
                ["dutyId"] = duty.Id,
                ["assignee"] = assignee
            }
        });
    }

    private async Task TransitionToNextAsync(
        CaseInstance instance,
        WorkflowDefinition workflow,
        WorkflowStepDefinition step,
        ExecutionContext context,
        string? transitionName,
        string? actingUserId,
        CancellationToken cancellationToken)
    {
        var transition = SelectTransition(workflow, step.Name, context, transitionName);
        if (transition is null) {
            throw new InvalidOperationException($"Step '{step.Name}' cannot transition to the next step.");
        }

        var transitionContext = new TransitionContext {
            Case = instance,
            Workflow = workflow,
            FromStep = transition.FromStep,
            ToStep = transition.ToStep,
            ActingUserId = actingUserId
        };

        if (workflow.Hooks.BeforeTransition is not null) {
            await workflow.Hooks.BeforeTransition(transitionContext, cancellationToken);
        }

        instance.CurrentStep = transition.ToStep;
        instance.Tracks.Add(new TrackEntry {
            Type = TrackType.StepChanged,
            Message = $"Moved from '{transition.FromStep}' to '{transition.ToStep}'.",
            Data = new Dictionary<string, object?> {
                ["fromStep"] = transition.FromStep,
                ["toStep"] = transition.ToStep,
                ["transitionName"] = transition.Name
            }
        });

        if (workflow.Hooks.AfterTransition is not null) {
            await workflow.Hooks.AfterTransition(transitionContext, cancellationToken);
        }
    }

    private static WorkflowTransitionDefinition? SelectTransition(
        WorkflowDefinition workflow,
        string currentStep,
        ExecutionContext context,
        string? transitionName)
    {
        var transitions = workflow.GetTransitionsFrom(currentStep);

        if (!string.IsNullOrWhiteSpace(transitionName)) {
            return transitions.SingleOrDefault(transition =>
                transition.IsManual &&
                string.Equals(transition.Name, transitionName, StringComparison.Ordinal));
        }

        var automaticTransitions = transitions.Where(transition => !transition.IsManual).ToArray();
        return automaticTransitions.FirstOrDefault(transition => transition.Condition?.Invoke(context) ?? true);
    }
}
