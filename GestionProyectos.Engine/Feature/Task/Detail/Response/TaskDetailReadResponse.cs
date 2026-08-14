namespace GestionProyectos.Engine.Feature.Task.Detail.Response;



public class TaskDetailReadResponse

{

    public TaskDetailInfo Task { get; set; } = new();

    public List<TaskStatusHistoryItem> StatusHistory { get; set; } = new();

}



public class TaskDetailInfo

{

    public long Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public long RequirementId { get; set; }

    public string RequirementDescription { get; set; } = string.Empty;

    public string RequirementStatus { get; set; } = string.Empty;

    public DateTime RequirementStartDate { get; set; }

    public DateTime? RequirementEndDate { get; set; }

    public long ProjectId { get; set; }

    public string ProjectDescription { get; set; } = string.Empty;

    public string CustomerDescription { get; set; } = string.Empty;

    public long UserId { get; set; }

    public string Responsible { get; set; } = string.Empty;

    public long PriorityId { get; set; }

    public string Priority { get; set; } = string.Empty;

    public string PriorityBadgeColor { get; set; } = "gray";

    public long TaskStatusId { get; set; }

    public string TaskStatus { get; set; } = string.Empty;

    public string TaskStatusBadgeColor { get; set; } = "gray";

    public long DevelopmentPhaseId { get; set; }

    public string DevelopmentPhase { get; set; } = string.Empty;

    public decimal TimeEstimationHours { get; set; }

    public decimal TotalHoursWorked { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? ActualEndDate { get; set; }

    public bool IsWithinOriginalScope { get; set; }

    public short? ScopeChangeReason { get; set; }

    public int BugCount { get; set; }

    public int NoteCount { get; set; }

}



public class TaskStatusHistoryItem

{

    public long Id { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? PreviousStatus { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string ChangedBy { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; }

}

