namespace MiniBP.BPMS.Abstractions;

public enum CaseStatus
{
    Draft,
    ToDo,
    Done,
    Cancelled,
    Paused
}

public enum WorkflowStepType
{
    Start,
    End,
    User,
    Service,
    Gateway
}

public enum TrackType
{
    CaseStarted,
    StepChanged,
    DutyCreated,
    DutyClaimed,
    DutyCompleted,
    CommentAdded,
    ParameterChanged,
    CaseCompleted
}
