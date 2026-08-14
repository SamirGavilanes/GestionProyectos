namespace GestionProyectos.Engine.Feature.Billing.Response;

public class BillingReportResponse
{
    public List<BillingHourTypeColumn> HourTypes { get; set; } = new();
    public List<BillingTimeLogRow> Rows { get; set; } = new();
    public decimal TotalHours { get; set; }
}

public class BillingHourTypeColumn
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BadgeColor { get; set; } = "gray";
    public decimal TotalHours { get; set; }
}

public class BillingTimeLogRow
{
    public long TimeLogId { get; set; }
    public DateTime ExecutionDate { get; set; }
    public decimal UsedHours { get; set; }
    public decimal ProgressPercent { get; set; }
    public long HourTypeId { get; set; }
    public string HourTypeName { get; set; } = string.Empty;
    public string HourTypeBadgeColor { get; set; } = "gray";
    public long TaskId { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public long RequirementId { get; set; }
    public string RequirementDescription { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectDescription { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string EnterpriseName { get; set; } = string.Empty;
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}
