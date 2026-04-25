namespace MiniBP.Samples.Api.Contracts;

public sealed record StartWorkflowCaseRequest(Dictionary<string, object?> Parameters);

public sealed record ClaimDutyRequest(string UserId);

public sealed record CompleteDutyApiRequest(string UserId, string? TransitionName, string? Comment, Dictionary<string, object?> Parameters);

public sealed record AddCommentRequest(string UserId, string Text);
