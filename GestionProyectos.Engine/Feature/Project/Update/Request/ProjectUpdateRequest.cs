using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Project.Update.Request
{
    public class ProjectUpdateRequest
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long ProjectStatusId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public Context Context { get; set; } = null!;
        public ProjectUpdateRequest()
        {
        }
    }
}
