namespace GestionProyectos.Engine.Feature.Task.TimeLogList.Response;

public class TaskTimeLogListResponse
{
    public long TaskId { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public decimal TimeEstimationHours { get; set; }
    public decimal TotalHoursWorked { get; set; }
    public decimal CurrentProgressPercent { get; set; }
    public List<TaskTimeLogListItem> Items { get; set; } = new();
}

public class TaskTimeLogListItem
{
    public long Id { get; set; }
    public DateTime ExecutionDate { get; set; }
    public decimal UsedHours { get; set; }
    public decimal ProgressPercent { get; set; }
    public decimal ProgressDelta { get; set; }
    public string UserName { get; set; } = string.Empty;
    public long HourTypeId { get; set; }
    public string HourTypeName { get; set; } = string.Empty;
    public string HourTypeBadgeColor { get; set; } = "gray";
}
