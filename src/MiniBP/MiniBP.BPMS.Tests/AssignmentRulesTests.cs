using MiniBP.BPMS.Abstractions;

namespace MiniBP.BPMS.Tests;

public sealed class AssignmentRulesTests
{
    [Fact]
    public void Cyclic_Assignment_Rotates_Through_Users()
    {
        var rule = AssignmentRules.Cyclic("u1", "u2", "u3");
        var context = new AssignmentContext {
            Case = new CaseInstance {
                WorkflowName = "Test",
                CurrentStep = "Step"
            },
            Step = new WorkflowStepDefinition {
                Name = "Step",
                Type = WorkflowStepType.User
            },
            Services = EmptyServiceProvider.Instance
        };

        Assert.Equal("u1", rule.ResolveAssignee(context));
        Assert.Equal("u2", rule.ResolveAssignee(context));
        Assert.Equal("u3", rule.ResolveAssignee(context));
        Assert.Equal("u1", rule.ResolveAssignee(context));
    }

    [Fact]
    public void Claiming_Assignment_Returns_Null_Assignee()
    {
        var rule = AssignmentRules.Claiming();
        var context = new AssignmentContext {
            Case = new CaseInstance {
                WorkflowName = "Test",
                CurrentStep = "Step"
            },
            Step = new WorkflowStepDefinition {
                Name = "Step",
                Type = WorkflowStepType.User
            },
            Services = EmptyServiceProvider.Instance
        };

        Assert.Null(rule.ResolveAssignee(context));
    }
}
