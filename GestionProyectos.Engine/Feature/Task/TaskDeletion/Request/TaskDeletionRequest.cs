using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Task.TaskDeletion.Request
{
    public class TaskDeletionRequest
    {
        public long TaskId { get; set; }
        public Context Context { get; set; } = null!;
    }
}
