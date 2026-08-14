using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Models.UploadFile;

namespace GestionProyectos.Engine.Feature.Task.Bug.Request;

public class TaskBugSaveRequest
{
    public long Id { get; set; }
    public long RequirementId { get; set; }
    public long? TaskId { get; set; }
    public string Description { get; set; } = string.Empty;
    public long TaskBugStatusId { get; set; }
    public List<FileItem> Files { get; set; } = new();
    public Context Context { get; set; } = new();
}

public class TaskBugAssignRequest
{
    public long BugId { get; set; }
    public long TaskId { get; set; }
    public Context Context { get; set; } = new();
}

public class TaskBugListRequest
{
    public long TaskId { get; set; }
    public Context Context { get; set; } = new();
}

public class TaskBugGlobalListRequest
{
    public long TaskBugStatusId { get; set; }
    public long EnterpriseId { get; set; }
    public long CustomerId { get; set; }
    public long ProjectId { get; set; }
    public long DeveloperId { get; set; }
    public Context Context { get; set; } = new();
}

public class TaskBugDeleteRequest
{
    public long Id { get; set; }
    public Context Context { get; set; } = new();
}

public class TaskBugDownloadRequest
{
    public long AttachmentId { get; set; }
    public Context Context { get; set; } = new();
}
