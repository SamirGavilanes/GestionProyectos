using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Project.Detail.Request;

public class ProjectDetailReadRequest
{
    public long ProjectId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today;
    public long TaskStatusId { get; set; }
    public long RequirementStatusId { get; set; }
    public long ResponsibleUserId { get; set; }
    public Context Context { get; set; } = new();
}