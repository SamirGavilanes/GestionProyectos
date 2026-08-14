using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Project.Create.Request
{
    public class ProjectCreateRequest
    {
        public string Description { get; set; } = string.Empty;
        public long CustomerId { get; set; }
        public long ProjectStatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public Context Context { get; set; } = null!;
        public ProjectCreateRequest()
        {
        }
    }
}
