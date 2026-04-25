using MiniBP.BPMS.Abstractions;

namespace MiniBP.Samples.Api.Workflows;

public static class LoanApplicationWorkflow
{
    public static WorkflowDefinition Build() =>
        WorkflowBuilder
            .Create("LoanApplication")
            .StartWith("Start")
            .Then("SubmitApplication")
            .UserStep("SubmitApplication", step => step
                .Title("Submit loan application")
                .AssignmentRule(AssignmentRules.Claiming())
                .Priority(10))
            .Then("InitialAssessment")
            .ServiceStep("InitialAssessment", step => step.Run((context, _) => {
                var amount = context.GetParameter<decimal>("amount");
                context.FlowParameters["requiresManagerApproval"] = amount >= 5_000m;
                return Task.CompletedTask;
            }))
            .Then("AmountGateway")
            .Gateway("AmountGateway")
            .If(context => context.GetParameter<bool>("requiresManagerApproval"))
            .Then("ManagerApproval")
            .Else()
            .Then("Archive")
            .UserStep("ManagerApproval", step => step
                .Title("Manager approval")
                .AssignmentRule(AssignmentRules.Cyclic("manager-a", "manager-b"))
                .Priority(100))
            .Then("Archive")
            .ManualTransition("Reject")
            .To("Rejected")
            .EndWith("Rejected")
            .EndWith("Archive")
            .OnBeforeTransition((context, _) => {
                context.Case.FlowParameters["lastTransitionAtUtc"] = DateTimeOffset.UtcNow;
                return Task.CompletedTask;
            })
            .Build();
}
