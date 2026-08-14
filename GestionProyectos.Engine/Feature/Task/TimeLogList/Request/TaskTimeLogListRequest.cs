using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Task.TimeLogList.Request;

public class TaskTimeLogListRequest
{
    public long TaskId { get; set; }
    public Context Context { get; set; } = null!;
}
