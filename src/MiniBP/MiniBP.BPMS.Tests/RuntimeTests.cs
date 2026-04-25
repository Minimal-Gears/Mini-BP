using MiniBP.BPMS.Abstractions;

namespace MiniBP.BPMS.Tests;

public sealed class RuntimeTests
{
    [Fact]
    public async Task StartCase_AutoAdvances_Through_Service_And_Completes_End_Step()
    {
        var workflow = WorkflowBuilder
            .Create("AutoComplete")
            .StartWith("Start")
            .Then("Compute")
            .ServiceStep("Compute", step => step.Run((context, _) => {
                context.FlowParameters["computed"] = true;
                return Task.CompletedTask;
            }))
            .Then("Done")
            .EndWith("Done")
            .Build();

        var harness = TestHarness.Create(workflow);

        var instance = await harness.CaseService.StartCaseAsync(new StartCaseRequest {
            WorkflowName = workflow.Name
        });

        Assert.Equal(CaseStatus.Done, instance.Status);
        Assert.Equal("Done", instance.CurrentStep);
        Assert.Equal(true, instance.FlowParameters["computed"]);
        Assert.Contains(instance.Tracks, track => track.Type == TrackType.CaseCompleted);
    }

    [Fact]
    public async Task StartCase_Creates_Claimable_Duty_For_User_Step()
    {
        var workflow = WorkflowBuilder
            .Create("ClaimableDuty")
            .StartWith("Start")
            .Then("Review")
            .UserStep("Review", step => step
                .Title("Review application")
                .AssignmentRule(AssignmentRules.Claiming())
                .Priority(5))
            .Then("Done")
            .EndWith("Done")
            .Build();

        var harness = TestHarness.Create(workflow);

        var instance = await harness.CaseService.StartCaseAsync(new StartCaseRequest {
            WorkflowName = workflow.Name
        });
        var duty = Assert.Single(await harness.DutyService.GetInboxAsync());

        Assert.Equal(CaseStatus.ToDo, instance.Status);
        Assert.Equal("Review", instance.CurrentStep);
        Assert.Equal("Review application", duty.Title);
        Assert.Null(duty.Assignee);
        Assert.Equal(instance.Id, duty.CaseId);
    }

    [Fact]
    public async Task CompleteDuty_Uses_Manual_Transition_When_Requested()
    {
        var workflow = WorkflowBuilder
            .Create("ManualDecision")
            .StartWith("Start")
            .Then("Review")
            .UserStep("Review", step => step.Title("Review"))
            .ManualTransition("Reject")
            .To("Rejected")
            .Then("Approved")
            .EndWith("Approved")
            .EndWith("Rejected")
            .Build();

        var harness = TestHarness.Create(workflow);
        var instance = await harness.CaseService.StartCaseAsync(new StartCaseRequest {
            WorkflowName = workflow.Name
        });
        var duty = Assert.Single(await harness.DutyService.GetInboxAsync());

        await harness.DutyService.CompleteDutyAsync(duty.Id, new CompleteDutyRequest {
            CompletedByUserId = "reviewer-1",
            TransitionName = "Reject"
        });

        var updatedCase = await harness.CaseService.GetCaseAsync(instance.Id);

        Assert.NotNull(updatedCase);
        Assert.Equal(CaseStatus.Done, updatedCase.Status);
        Assert.Equal("Rejected", updatedCase.CurrentStep);
        Assert.Contains(updatedCase.Tracks, track => track.Type == TrackType.DutyCompleted);
    }

    [Fact]
    public async Task CompleteDuty_Evaluates_Gateway_And_Creates_Next_Assigned_Duty()
    {
        var workflow = WorkflowBuilder
            .Create("GatewayFlow")
            .StartWith("Start")
            .Then("Submit")
            .UserStep("Submit", step => step.Title("Submit request"))
            .Then("CheckAmount")
            .Gateway("CheckAmount")
            .If(context => context.GetParameter<decimal>("amount") >= 1000m)
            .Then("ManagerApproval")
            .Else()
            .Then("Archive")
            .UserStep("ManagerApproval", step => step
                .Title("Manager approval")
                .AssignmentRule(AssignmentRules.Fixed("manager-1")))
            .Then("Archive")
            .EndWith("Archive")
            .Build();

        var harness = TestHarness.Create(workflow);
        await harness.CaseService.StartCaseAsync(new StartCaseRequest {
            WorkflowName = workflow.Name
        });
        var firstDuty = Assert.Single(await harness.DutyService.GetInboxAsync());

        await harness.DutyService.CompleteDutyAsync(firstDuty.Id, new CompleteDutyRequest {
            CompletedByUserId = "submitter-1",
            Parameters = new Dictionary<string, object?> {
                ["amount"] = 1500m
            }
        });

        var duties = await harness.DutyService.GetInboxAsync();
        var managerDuty = Assert.Single(duties);

        Assert.Equal("ManagerApproval", managerDuty.StepName);
        Assert.Equal("manager-1", managerDuty.Assignee);
    }

    [Fact]
    public async Task AddComment_Stores_Comment_On_Duty()
    {
        var workflow = WorkflowBuilder
            .Create("CommentFlow")
            .StartWith("Start")
            .Then("Review")
            .UserStep("Review", step => step.Title("Review"))
            .Then("Done")
            .EndWith("Done")
            .Build();

        var harness = TestHarness.Create(workflow);
        await harness.CaseService.StartCaseAsync(new StartCaseRequest {
            WorkflowName = workflow.Name
        });
        var duty = Assert.Single(await harness.DutyService.GetInboxAsync());

        await harness.DutyService.AddCommentAsync(duty.Id, "reviewer-1", "Looks good.");
        var updatedDuty = await harness.DutyService.GetDutyAsync(duty.Id);

        Assert.NotNull(updatedDuty);
        var comment = Assert.Single(updatedDuty.Comments);
        Assert.Equal("reviewer-1", comment.AuthorUserId);
        Assert.Equal("Looks good.", comment.Text);
    }

    private sealed class TestHarness(
        ICaseService caseService,
        IDutyService dutyService)
    {
        public ICaseService CaseService { get; } = caseService;
        public IDutyService DutyService { get; } = dutyService;

        public static TestHarness Create(params WorkflowDefinition[] workflows)
        {
            var registry = new InMemoryWorkflowRegistry();
            foreach (var workflow in workflows) {
                registry.Register(workflow);
            }

            var store = new InMemoryRuntimeStore();
            var services = EmptyServiceProvider.Instance;

            return new TestHarness(
                new CaseService(registry, store, services),
                new DutyService(registry, store, services));
        }
    }
}
