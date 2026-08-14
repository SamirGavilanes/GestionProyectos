using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Task.Detail.Request;

public class TaskDetailReadRequest
{
    public long TaskId { get; set; }
    public Context Context { get; set; } = new();
}
