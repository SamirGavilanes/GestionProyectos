using GestionProyectos.Engine.Security.Utilities;



namespace GestionProyectos.Engine.Feature.Task.TaskCreation.Request

{

    public class TaskCreationRequest

    {

        public long TicketId { get; set; }

        public string? Description { get; set; }

        public decimal TimeEstimationHours { get; set; }
        public bool IsWithinOriginalScope { get; set; } = true;
        public short? ScopeChangeReason { get; set; }
        public long DevelopmentPhaseId { get; set; }
        public long UserId { get; set; }
        public Context Context { get; set; } = null!;

        public TaskCreationRequest()

        {

        }

    }

}

