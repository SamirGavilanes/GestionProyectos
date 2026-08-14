using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.HoursReport.Request;

public class HoursReportReadRequest
{
    public Context Context { get; set; } = new();
    public int Year { get; set; }
    public int Month { get; set; }
}
