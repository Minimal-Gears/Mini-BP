using System.Collections.ObjectModel;

namespace MiniBP.BPMS.Abstractions;

public sealed class WorkflowBuilder
{
    private readonly Dictionary<string, StepBuilderState> _steps = new(StringComparer.Ordinal);
    private readonly List<WorkflowTransitionDefinition> _transitions = [];
    private readonly string _name;
    private string? _currentStep;
    private Func<ExecutionContext, bool>? _pendingCondition;
    private Func<TransitionContext, CancellationToken, Task>? _beforeTransition;
    private Func<TransitionContext, CancellationToken, Task>? _afterTransition;

    private WorkflowBuilder(string name)
    {
        _name = name;
    }

    public static WorkflowBuilder Create(string name) => new(name);

    public WorkflowBuilder StartWith(string stepName)
    {
        AddOrUpdateStep(stepName, WorkflowStepType.Start, _ => { });
        _currentStep = stepName;
        return this;
    }

    public WorkflowBuilder EndWith(string stepName)
    {
        AddOrUpdateStep(stepName, WorkflowStepType.End, _ => { });
        _currentStep = stepName;
        return this;
    }

    public WorkflowBuilder UserStep(string stepName, Action<UserStepBuilder>? configure = null)
    {
        AddOrUpdateStep(stepName, WorkflowStepType.User, state => configure?.Invoke(new UserStepBuilder(state)));
        _currentStep = stepName;
        return this;
    }

    public WorkflowBuilder ServiceStep(string stepName, Action<ServiceStepBuilder>? configure = null)
    {
        AddOrUpdateStep(stepName, WorkflowStepType.Service, state => configure?.Invoke(new ServiceStepBuilder(state)));
        _currentStep = stepName;
        return this;
    }

    public WorkflowBuilder Gateway(string stepName)
    {
        AddOrUpdateStep(stepName, WorkflowStepType.Gateway, _ => { });
        _currentStep = stepName;
        return this;
    }

    public WorkflowBuilder Then(string nextStep)
    {
        EnsureCurrentStep();
        _transitions.Add(new WorkflowTransitionDefinition {
            FromStep = _currentStep!,
            ToStep = nextStep,
            Condition = _pendingCondition
        });
        _pendingCondition = null;
        return this;
    }

    public WorkflowBuilder If(Func<ExecutionContext, bool> condition)
    {
        _pendingCondition = condition;
        return this;
    }

    public WorkflowBuilder Else()
    {
        _pendingCondition = null;
        return this;
    }

    public ManualTransitionBuilder ManualTransition(string name)
    {
        EnsureCurrentStep();
        return new ManualTransitionBuilder(this, _currentStep!, name);
    }

    public WorkflowBuilder OnBeforeTransition(Func<TransitionContext, CancellationToken, Task> callback)
    {
        _beforeTransition = callback;
        return this;
    }

    public WorkflowBuilder OnAfterTransition(Func<TransitionContext, CancellationToken, Task> callback)
    {
        _afterTransition = callback;
        return this;
    }

    public WorkflowDefinition Build()
    {
        Validate();

        return new WorkflowDefinition {
            Name = _name,
            Steps = new ReadOnlyDictionary<string, WorkflowStepDefinition>(
                _steps.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Build(),
                    StringComparer.Ordinal)),
            Transitions = _transitions.ToArray(),
            Hooks = new WorkflowHooks {
                BeforeTransition = _beforeTransition,
                AfterTransition = _afterTransition
            }
        };
    }

    internal WorkflowBuilder AddManualTransition(string fromStep, string name, string toStep)
    {
        _transitions.Add(new WorkflowTransitionDefinition {
            FromStep = fromStep,
            ToStep = toStep,
            Name = name,
            IsManual = true
        });
        return this;
    }

    private void AddOrUpdateStep(string name, WorkflowStepType type, Action<StepBuilderState> configure)
    {
        if (!_steps.TryGetValue(name, out var state)) {
            state = new StepBuilderState(name, type);
            _steps[name] = state;
        }
        else if (state.Type != type) {
            throw new InvalidOperationException($"Step '{name}' is already defined as {state.Type}.");
        }

        configure(state);
    }

    private void EnsureCurrentStep()
    {
        if (string.IsNullOrWhiteSpace(_currentStep)) {
            throw new InvalidOperationException("A step must be selected before transitions can be added.");
        }
    }

    private void Validate()
    {
        if (_steps.Values.Count(step => step.Type == WorkflowStepType.Start) != 1) {
            throw new InvalidOperationException("Workflow must contain exactly one start step.");
        }

        if (_steps.Values.All(step => step.Type != WorkflowStepType.End)) {
            throw new InvalidOperationException("Workflow must contain at least one end step.");
        }

        foreach (var transition in _transitions) {
            if (!_steps.ContainsKey(transition.ToStep)) {
                throw new InvalidOperationException($"Transition target step '{transition.ToStep}' does not exist.");
            }
        }

        foreach (var gateway in _steps.Values.Where(step => step.Type == WorkflowStepType.Gateway)) {
            if (_transitions.All(transition => !string.Equals(transition.FromStep, gateway.Name, StringComparison.Ordinal))) {
                throw new InvalidOperationException($"Gateway '{gateway.Name}' does not define any transition.");
            }
        }
    }

    internal sealed class StepBuilderState(string name, WorkflowStepType type)
    {
        public string Name { get; } = name;
        public WorkflowStepType Type { get; } = type;
        public string? Title { get; set; }
        public int Priority { get; set; }
        public IAssignmentRule AssignmentRule { get; set; } = AssignmentRules.Claiming();
        public Func<ExecutionContext, CancellationToken, Task>? ServiceAction { get; set; }
        public Func<ExecutionContext, CancellationToken, Task>? BeforeDutyExecution { get; set; }
        public Func<ExecutionContext, CancellationToken, Task>? AfterDutyExecution { get; set; }

        public WorkflowStepDefinition Build() =>
            new() {
                Name = Name,
                Type = Type,
                Title = Title,
                Priority = Priority,
                AssignmentRule = AssignmentRule,
                ServiceAction = ServiceAction,
                Hooks = new StepHooks {
                    BeforeDutyExecution = BeforeDutyExecution,
                    AfterDutyExecution = AfterDutyExecution
                }
            };
    }

    public sealed class UserStepBuilder
    {
        private readonly StepBuilderState _state;

        internal UserStepBuilder(StepBuilderState state)
        {
            _state = state;
        }

        public UserStepBuilder Title(string title)
        {
            _state.Title = title;
            return this;
        }

        public UserStepBuilder Priority(int priority)
        {
            _state.Priority = priority;
            return this;
        }

        public UserStepBuilder AssignmentRule(IAssignmentRule rule)
        {
            _state.AssignmentRule = rule;
            return this;
        }

        public UserStepBuilder BeforeDutyExecution(Func<ExecutionContext, CancellationToken, Task> callback)
        {
            _state.BeforeDutyExecution = callback;
            return this;
        }

        public UserStepBuilder AfterDutyExecution(Func<ExecutionContext, CancellationToken, Task> callback)
        {
            _state.AfterDutyExecution = callback;
            return this;
        }
    }

    public sealed class ServiceStepBuilder
    {
        private readonly StepBuilderState _state;

        internal ServiceStepBuilder(StepBuilderState state)
        {
            _state = state;
        }

        public ServiceStepBuilder Run(Func<ExecutionContext, CancellationToken, Task> action)
        {
            _state.ServiceAction = action;
            return this;
        }
    }

    public sealed class ManualTransitionBuilder(WorkflowBuilder workflow, string fromStep, string name)
    {
        public WorkflowBuilder To(string targetStep) => workflow.AddManualTransition(fromStep, name, targetStep);
    }
}
