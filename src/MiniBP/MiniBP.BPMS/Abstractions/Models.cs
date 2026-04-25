namespace MiniBP.BPMS.Abstractions;

public sealed class CaseInstance
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string WorkflowName { get; init; }
    public required string CurrentStep { get; set; }
    public CaseStatus Status { get; set; } = CaseStatus.Draft;
    public Dictionary<string, object?> FlowParameters { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<TrackEntry> Tracks { get; } = [];
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAtUtc { get; set; }
}

public sealed class Duty
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CaseId { get; init; }
    public required string StepName { get; init; }
    public required string Title { get; init; }
    public string? Assignee { get; set; }
    public int Priority { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAtUtc { get; set; }
    public List<DutyComment> Comments { get; } = [];
    public bool IsCompleted => CompletedAtUtc.HasValue;
}

public sealed class DutyComment
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string AuthorUserId { get; init; }
    public required string Text { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class TrackEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required TrackType Type { get; init; }
    public required string Message { get; init; }
    public Dictionary<string, object?> Data { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}
