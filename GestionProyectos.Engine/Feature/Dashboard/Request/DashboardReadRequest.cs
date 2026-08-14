using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Dashboard.Request;

public class DashboardReadRequest
{
    public Context Context { get; set; } = new();
    public long ProjectId { get; set; }
    public long TaskStatusId { get; set; }
    public long BugStatusId { get; set; }
    public DateTime? WorkStartDate { get; set; }
    public DateTime? WorkEndDate { get; set; }
}
