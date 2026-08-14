using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Task.Note.Request;

public class TaskNoteListRequest
{
    public long TaskId { get; set; }
    public Context Context { get; set; } = new();
}

public class TaskNoteSaveRequest
{
    public long TaskId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Context Context { get; set; } = new();
}

public class TaskNoteDeleteRequest
{
    public long Id { get; set; }
    public Context Context { get; set; } = new();
}
