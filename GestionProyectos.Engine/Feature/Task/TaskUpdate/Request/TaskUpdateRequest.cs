using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Task.TaskUpdate.Request
{
    public class TaskUpdateRequest
    {
        public long Id { get; set; }
        public long TicketId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal TimeEstimationHours { get; set; }
        public long TaskStatusId { get; set; }
        public bool IsWithinOriginalScope { get; set; } = true;
        public short? ScopeChangeReason { get; set; }
        public string? StatusChangeReason { get; set; }
        public long DevelopmentPhaseId { get; set; }
        public long? UserId { get; set; }
        public Context Context { get; set; } = null!;
        public TaskUpdateRequest()
        {
        }
    }
}
