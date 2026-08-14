namespace GestionProyectos.Engine.Feature.Project.Detail.Response;

public class ProjectDetailReadResponse
{
    public ProjectDetailInfo Project { get; set; } = new();
    public List<ProjectDetailRequirementItem> Requirements { get; set; } = new();
    public ProjectDetailSummary Summary { get; set; } = new();
    public decimal TotalEstimatedHours { get; set; }
    public decimal TotalLoggedHours { get; set; }
}

public class ProjectDetailSummary
{
    public decimal EstimatedHours { get; set; }
    public decimal LoggedHours { get; set; }
    public decimal RemainingHours { get; set; }
    public decimal LoggedPercent { get; set; }
    public decimal RemainingPercent { get; set; }

    public decimal HoursNegativeVariance { get; set; }
    public decimal HoursPositiveVariance { get; set; }
    public decimal AverageHoursVariance { get; set; }
    public decimal HoursNegativeVariancePercent { get; set; }
    public decimal HoursPositiveVariancePercent { get; set; }

    public int TasksNegativeVariance { get; set; }
    public int TasksPositiveVariance { get; set; }
    public int TasksOnTarget { get; set; }
    public int TotalTasks { get; set; }
    public decimal TasksNegativeVariancePercent { get; set; }
    public decimal TasksPositiveVariancePercent { get; set; }
    public decimal AverageTaskVariancePercent { get; set; }

    public decimal ProgressPercent { get; set; }
    public decimal ExpectedProgressPercent { get; set; }
    public decimal ScheduleVariancePercent { get; set; }
    public int TasksAheadSchedule { get; set; }
    public int TasksBehindSchedule { get; set; }
    public int TasksOnSchedule { get; set; }
    public decimal TasksAheadSchedulePercent { get; set; }
    public decimal TasksBehindSchedulePercent { get; set; }

    public ProjectDetailTaskStatusCounts TaskStatusCounts { get; set; } = new();
}

public class ProjectDetailTaskStatusCounts
{
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Finished { get; set; }
    public int InternalBlock { get; set; }
    public int ExternalBlock { get; set; }
}

public class ProjectDetailInfo
{
    public long Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsClosedStatus { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
}

public class ProjectDetailRequirementItem
{
    public long Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public long RequirementStatusId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal EstimatedHours { get; set; }
    public decimal LoggedHours { get; set; }
    public List<ProjectDetailTaskItem> Tasks { get; set; } = new();
}

public class ProjectDetailTaskItem
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public long TaskStatusId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal EstimatedHours { get; set; }
    public decimal LoggedHours { get; set; }
    public DateTime? LastHoursUpdateAt { get; set; }
    public DateTime? LastExecutionDate { get; set; }
    public decimal ProgressPercent { get; set; }
}
