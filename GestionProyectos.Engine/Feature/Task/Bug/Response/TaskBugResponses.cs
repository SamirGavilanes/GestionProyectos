namespace GestionProyectos.Engine.Feature.Task.Bug.Response;

public class TaskBugListResponse
{
    public List<TaskBugItem> Bugs { get; set; } = new();
}

public class TaskBugGlobalListResponse
{
    public List<TaskBugOverviewItem> Bugs { get; set; } = new();
}

public class TaskBugOverviewItem : TaskBugItem
{
    public long RequirementId { get; set; }
    public string RequirementDescription { get; set; } = string.Empty;
    public bool IsWithinOriginalScope { get; set; } = true;
    public long? TaskId { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public long DeveloperId { get; set; }
    public string DeveloperName { get; set; } = string.Empty;
    public bool IsAssigned => TaskId.HasValue && TaskId.Value > 0;
}

public class TaskBugTaskOption
{
    public long Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string DeveloperName { get; set; } = string.Empty;
}

public class TaskBugTaskOptionsResponse
{
    public List<TaskBugTaskOption> Tasks { get; set; } = new();
}

public class TaskBugItem
{
    public long Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public long TaskBugStatusId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ReportedBy { get; set; } = string.Empty;
    public DateTime ReportedAt { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<TaskBugAttachmentItem> Attachments { get; set; } = new();
}

public class TaskBugAttachmentItem
{
    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
}

public class TaskBugDownloadResponse
{
    public string FileName { get; set; } = string.Empty;
    public byte[] File { get; set; } = Array.Empty<byte>();
}

public class TaskBugSaveResponse
{
    public long Id { get; set; }
}
