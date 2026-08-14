using GestionProyectos.Engine.Security.Utilities;

namespace GestionProyectos.Engine.Feature.Billing.Request;

public class BillingReportRequest
{
    public Context Context { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long EnterpriseId { get; set; }
    public long CustomerId { get; set; }
    public long ProjectId { get; set; }
    public long RequirementId { get; set; }
    public long HourTypeId { get; set; }
}
