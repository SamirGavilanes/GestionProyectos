using GestionProyectos.Engine.Feature.Billing.Response;

namespace GestionProyectos.Engine.Feature.Billing.Request;

public enum BillingExportMode
{
    Raw,
    Pivot
}

public enum BillingPivotGroupBy
{
    Project,
    Requirement,
    Date,
    HourType
}

public class BillingExportRequest
{
    public BillingExportMode Mode { get; set; } = BillingExportMode.Raw;
    public BillingPivotGroupBy PivotGroupBy { get; set; } = BillingPivotGroupBy.Project;
    public BillingReportResponse Data { get; set; } = new();
}
