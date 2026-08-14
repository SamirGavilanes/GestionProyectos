using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Performance.Request;

public class PerformanceReadRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool UseDateRange { get; set; }
    public Context Context { get; set; } = new();
    public long EnterpriseId { get; set; }
    public long CustomerId { get; set; }
    public long ProjectId { get; set; }
    public long RequirementId { get; set; }
    public long DeveloperId { get; set; }
}
