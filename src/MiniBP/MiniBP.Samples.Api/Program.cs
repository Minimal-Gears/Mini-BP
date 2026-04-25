using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MiniBP.BPMS.Abstractions;
using MiniBP.Samples.Api.Contracts;
using MiniBP.Samples.Api.Workflows;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryRuntimeStore>();
builder.Services.AddSingleton<IWorkflowRegistry, InMemoryWorkflowRegistry>();
builder.Services.AddSingleton<ICaseService, CaseService>();
builder.Services.AddSingleton<IDutyService, DutyService>();

var app = builder.Build();

var workflows = app.Services.GetRequiredService<IWorkflowRegistry>();
workflows.Register(LoanApplicationWorkflow.Build());

app.MapGet("/", () => TypedResults.Ok(new {
    name = "Mini-BP Sample API",
    workflow = "LoanApplication"
}));

app.MapGet("/workflows", (IWorkflowRegistry registry) =>
    TypedResults.Ok(registry.GetAll().Select(workflow => new {
        workflow.Name,
        Steps = workflow.Steps.Values.Select(step => new { step.Name, Type = step.Type.ToString(), step.Title })
    })));

app.MapPost("/cases", async (StartWorkflowCaseRequest request, ICaseService caseService, CancellationToken cancellationToken) =>
{
    var instance = await caseService.StartCaseAsync(new StartCaseRequest {
        WorkflowName = "LoanApplication",
        StartedByUserId = "sample-user",
        Parameters = request.Parameters
    }, cancellationToken);

    return TypedResults.Created($"/cases/{instance.Id}", instance);
});

app.MapGet("/cases", async (ICaseService caseService, CancellationToken cancellationToken) =>
    TypedResults.Ok(await caseService.GetCasesAsync(cancellationToken)));

app.MapGet("/cases/{caseId:guid}", async Task<IResult> (Guid caseId, ICaseService caseService, CancellationToken cancellationToken) =>
{
    var instance = await caseService.GetCaseAsync(caseId, cancellationToken);
    return instance is null ? Results.NotFound() : Results.Ok(instance);
});

app.MapGet("/duties", async (string? userId, IDutyService dutyService, CancellationToken cancellationToken) =>
    TypedResults.Ok(await dutyService.GetInboxAsync(userId, cancellationToken)));

app.MapGet("/duties/{dutyId:guid}", async Task<IResult> (Guid dutyId, IDutyService dutyService, CancellationToken cancellationToken) =>
{
    var duty = await dutyService.GetDutyAsync(dutyId, cancellationToken);
    return duty is null ? Results.NotFound() : Results.Ok(duty);
});

app.MapPost("/duties/{dutyId:guid}/claim", async (Guid dutyId, ClaimDutyRequest request, IDutyService dutyService, CancellationToken cancellationToken) =>
    TypedResults.Ok(await dutyService.ClaimDutyAsync(dutyId, request.UserId, cancellationToken)));

app.MapPost("/duties/{dutyId:guid}/complete", async (Guid dutyId, CompleteDutyApiRequest request, IDutyService dutyService, CancellationToken cancellationToken) =>
    TypedResults.Ok(await dutyService.CompleteDutyAsync(dutyId, new CompleteDutyRequest {
        CompletedByUserId = request.UserId,
        TransitionName = request.TransitionName,
        Comment = request.Comment,
        Parameters = request.Parameters
    }, cancellationToken)));

app.MapPost("/duties/{dutyId:guid}/comments", async (Guid dutyId, AddCommentRequest request, IDutyService dutyService, CancellationToken cancellationToken) =>
    TypedResults.Ok(await dutyService.AddCommentAsync(dutyId, request.UserId, request.Text, cancellationToken)));

app.Run();
