using MiniBP.BPMS.Abstractions;

namespace MiniBP.BPMS.Tests;

public sealed class WorkflowBuilderTests
{
    [Fact]
    public void Build_Throws_When_No_End_Step_Exists()
    {
        var builder = WorkflowBuilder
            .Create("InvalidWorkflow")
            .StartWith("Start")
            .Then("Review")
            .UserStep("Review", step => step.Title("Review"));

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());

        Assert.Equal("Workflow must contain at least one end step.", exception.Message);
    }

    [Fact]
    public void Build_Throws_When_Transition_Target_Does_Not_Exist()
    {
        var builder = WorkflowBuilder
            .Create("BrokenTransition")
            .StartWith("Start")
            .Then("MissingStep")
            .EndWith("Archive");

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());

        Assert.Equal("Transition target step 'MissingStep' does not exist.", exception.Message);
    }

    [Fact]
    public void Build_Creates_Manual_Transitions_And_Hooks()
    {
        var beforeCalled = false;
        var afterCalled = false;

        var workflow = WorkflowBuilder
            .Create("ManualFlow")
            .StartWith("Start")
            .Then("Review")
            .UserStep("Review", step => step.Title("Review duty"))
            .ManualTransition("Reject")
            .To("Rejected")
            .Then("Approved")
            .EndWith("Approved")
            .EndWith("Rejected")
            .OnBeforeTransition((_, _) => {
                beforeCalled = true;
                return Task.CompletedTask;
            })
            .OnAfterTransition((_, _) => {
                afterCalled = true;
                return Task.CompletedTask;
            })
            .Build();

        var manualTransition = Assert.Single(workflow.Transitions, item => item.IsManual);
        Assert.Equal("Reject", manualTransition.Name);
        Assert.Equal("Review", manualTransition.FromStep);
        Assert.Equal("Rejected", manualTransition.ToStep);
        Assert.NotNull(workflow.Hooks.BeforeTransition);
        Assert.NotNull(workflow.Hooks.AfterTransition);
        Assert.False(beforeCalled);
        Assert.False(afterCalled);
    }
}
