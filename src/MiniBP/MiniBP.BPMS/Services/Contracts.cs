namespace MiniBP.BPMS.Abstractions;

public interface IWorkflowRegistry
{
    void Register(WorkflowDefinition workflow);
    WorkflowDefinition Get(string workflowName);
    IReadOnlyCollection<WorkflowDefinition> GetAll();
}

public interface ICaseService
{
    Task<CaseInstance> StartCaseAsync(StartCaseRequest request, CancellationToken cancellationToken = default);
    Task<CaseInstance?> GetCaseAsync(Guid caseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CaseInstance>> GetCasesAsync(CancellationToken cancellationToken = default);
}

public interface IDutyService
{
    Task<IReadOnlyCollection<Duty>> GetInboxAsync(string? userId = null, CancellationToken cancellationToken = default);
    Task<Duty?> GetDutyAsync(Guid dutyId, CancellationToken cancellationToken = default);
    Task<Duty> ClaimDutyAsync(Guid dutyId, string userId, CancellationToken cancellationToken = default);
    Task<Duty> CompleteDutyAsync(Guid dutyId, CompleteDutyRequest request, CancellationToken cancellationToken = default);
    Task<DutyComment> AddCommentAsync(Guid dutyId, string userId, string text, CancellationToken cancellationToken = default);
}
