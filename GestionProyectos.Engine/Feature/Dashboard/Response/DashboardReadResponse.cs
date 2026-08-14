namespace GestionProyectos.Engine.Feature.Dashboard.Response;

public class DashboardReadResponse
{
    public double MeanAbsoluteErrorHours { get; set; }
    public int TotalTasks { get; set; }
    public decimal BugHoursTotal { get; set; }
    public List<DashboardTaskStatusCount> TasksByStatus { get; set; } = new();
    public List<DashboardOvertimeAlert> OvertimeTasks { get; set; } = new();
    public List<DashboardDeviatedTask> TopDeviatedTasks { get; set; } = new();

    public List<DashboardMyTaskItem> MyAssignedTasks { get; set; } = new();
    public List<DashboardMyBugItem> MyAssignedBugs { get; set; } = new();
    public List<DashboardMyTaskItem> MyDelayedTasks { get; set; } = new();
    public List<DashboardMyTaskItem> MyWorkedTasks { get; set; } = new();
    public DashboardFilterOptions FilterOptions { get; set; } = new();
    public DateTime WorkStartDate { get; set; }
    public DateTime WorkEndDate { get; set; }
    public List<DashboardTimeOffBannerItem> OutToday { get; set; } = new();
    public List<DashboardTimeOffBannerItem> UpcomingVacations { get; set; } = new();
    public int AssignedProjectsCount { get; set; }
    public int WorkedProjectsCount { get; set; }
    public List<DashboardWorkedProjectSummary> WorkedProjects { get; set; } = new();
}

public class DashboardFilterOptions
{
    public List<DashboardFilterOption> Projects { get; set; } = new();
    public List<DashboardFilterOption> TaskStatuses { get; set; } = new();
    public List<DashboardFilterOption> BugStatuses { get; set; } = new();
}

public class DashboardFilterOption
{
    public long Id { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class DashboardTaskStatusCount
{
    public long StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string BadgeColor { get; set; } = "gray";
    public string ChartColor { get; set; } = "#9ca3af";
    public int Count { get; set; }
}

public class DashboardOvertimeAlert
{
    public long TaskId { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ResponsibleName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal OvertimeHours { get; set; }
}

public class DashboardDeviatedTask
{
    public long TaskId { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ResponsibleName { get; set; } = string.Empty;
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public double DeviationPercent { get; set; }
    public decimal AbsoluteDeviationHours { get; set; }
}

public class DashboardMyTaskItem
{
    public long TaskId { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public long TaskStatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string StatusBadgeColor { get; set; } = "gray";
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public int BugCount { get; set; }
    public decimal OvertimeHours { get; set; }
    public double DeviationPercent { get; set; }
    public DateTime? LastWorkDate { get; set; }
}

public class DashboardMyBugItem
{
    public long BugId { get; set; }
    public string Description { get; set; } = string.Empty;
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public long? TaskId { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public long TaskBugStatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string StatusBadgeColor { get; set; } = "gray";
    public DateTime ReportedAt { get; set; }
}

public class DashboardTimeOffBannerItem
{
    public long UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public short Type { get; set; }
    public string TypeLabel { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class DashboardWorkedProjectSummary
{
    public long ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal HoursInPeriod { get; set; }
}
