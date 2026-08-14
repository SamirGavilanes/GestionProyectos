using GestionProyectos.Engine.Security.Utilities;



namespace GestionProyectos.Engine.Feature.Task.TaskList.Request

{

    public class TaskListRequest

    {

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Context Context { get; set; } = null!;

        public long EnterpriseId { get; set; }

        public long CustomerId { get; set; }

        public long ProjectId { get; set; }

        public long DeveloperId { get; set; }

        public bool OnlyAssignedToMe { get; set; }

        public long RequirementId { get; set; }

        public long TaskStatusId { get; set; }

        public bool OnlyDelayed { get; set; }

        public bool OnlyWithBugs { get; set; }

        public TaskListRequest()

        {

        }

    }

}

