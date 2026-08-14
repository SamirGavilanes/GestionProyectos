namespace GestionProyectos.Engine.Feature.Performance.Response;

public class PerformanceReadResponse
{
    public List<PerformanceTaskPoint> Tasks { get; set; } = new();
    public List<PerformanceEmployeeSummary> Employees { get; set; } = new();
}

public class PerformanceTaskPoint
{
    public long TaskId { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public double DeviationPercent { get; set; }
    public int BugCount { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class PerformanceEmployeeSummary
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public double MeanAbsoluteErrorHours { get; set; }
    public int TaskCount { get; set; }
    public int AbsentDays { get; set; }
    public decimal LoggedHours { get; set; }
    public string Color { get; set; } = string.Empty;
}
