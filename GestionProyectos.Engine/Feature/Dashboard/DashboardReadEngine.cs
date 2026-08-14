using GestionProyectos.Data;
using GestionProyectos.Data.Entities.TaskManagement;
using GestionProyectos.Engine.Feature.Dashboard.Request;
using GestionProyectos.Engine.Feature.Dashboard.Response;
using GestionProyectos.Engine.Feature.Task;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.Dashboard;

public class DashboardReadEngine : IDashboardReadEngine
{
    private static readonly Dictionary<string, string> BadgeColorToChart = new(StringComparer.OrdinalIgnoreCase)
    {
        ["green"] = "#22c55e",
        ["blue"] = "#3b82f6",
        ["amber"] = "#f59e0b",
        ["red"] = "#ef4444",
        ["violet"] = "#8b5cf6",
        ["purple"] = "#a855f7",
        ["orange"] = "#f97316",
        ["emerald"] = "#10b981",
        ["gray"] = "#9ca3af"
    };

    private readonly DataDbContext dbContext;

    public DashboardReadEngine(DataDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public OperationResult<DashboardReadResponse> Execute(DashboardReadRequest request)
    {
        try
        {
            var userId = request.Context.UserId;
            if (userId <= 0)
                return OperationResult<DashboardReadResponse>.CreateFailureResult("No se pudo identificar al usuario en sesión.");

            var today = DateTime.Today;
            var rangeStart = request.WorkStartDate?.Date ?? new DateTime(today.Year, today.Month, 1);
            var rangeEnd = request.WorkEndDate?.Date ?? today;
            if (rangeEnd < rangeStart)
                rangeEnd = rangeStart;

            var taskStatuses = dbContext.TaskStatus
                .AsNoTracking()
                .Where(s => s.RowStatus == (short)RowStatus.Active)
                .OrderBy(s => s.Order)
                .ToList();

            var bugStatuses = dbContext.TaskBugStatus
                .AsNoTracking()
                .Where(s => s.RowStatus == (short)RowStatus.Active)
                .OrderBy(s => s.Order)
                .ToList();

            var finalizedStatusIds = taskStatuses
                .Where(s => s.Description.Contains("finaliz", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Id)
                .ToHashSet();

            var myTasks = dbContext.Task
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.TaskStatus)
                .Include(t => t.Requirement)
                    .ThenInclude(r => r.Project)
                .Where(t => t.RowStatus == (short)RowStatus.Active && t.UserId == userId)
                .ToList();

            var myTaskIds = myTasks.Select(t => t.Id).ToList();

            var timeLogsByTask = dbContext.TimeLog
                .AsNoTracking()
                .Where(tl => myTaskIds.Contains(tl.TaskId) && tl.RowStatus == (short)RowStatus.Active)
                .AsEnumerable()
                .GroupBy(tl => tl.TaskId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var actualHoursByTask = timeLogsByTask.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Sum(x => x.UsedHours));

            var bugCountByTask = dbContext.TaskBug
                .AsNoTracking()
                .Where(b => b.TaskId.HasValue && myTaskIds.Contains(b.TaskId.Value) && b.RowStatus == (short)RowStatus.Active)
                .AsEnumerable()
                .GroupBy(b => b.TaskId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            var qaEnteredByTask = myTasks.ToDictionary(t => t.Id, t => t.QaEnteredAt);
            var bugHoursByTask = timeLogsByTask.ToDictionary(
                kvp => kvp.Key,
                kvp => TaskPhaseHelper.CalculateBugHours(kvp.Value, qaEnteredByTask.GetValueOrDefault(kvp.Key)));

            var response = new DashboardReadResponse
            {
                TotalTasks = myTasks.Count,
                BugHoursTotal = bugHoursByTask.Values.Sum(),
                WorkStartDate = rangeStart,
                WorkEndDate = rangeEnd
            };

            var tasksWithEstimation = myTasks
                .Where(t => t.TimeEstimationHours > 0 && actualHoursByTask.GetValueOrDefault(t.Id) > 0)
                .ToList();

            if (tasksWithEstimation.Count > 0)
            {
                response.MeanAbsoluteErrorHours = Math.Round(
                    tasksWithEstimation.Average(t =>
                        Math.Abs((double)(actualHoursByTask[t.Id] - t.TimeEstimationHours))),
                    2);
            }

            foreach (var status in taskStatuses)
            {
                response.TasksByStatus.Add(new DashboardTaskStatusCount
                {
                    StatusId = status.Id,
                    StatusName = status.Description,
                    BadgeColor = status.BadgeColor,
                    ChartColor = BadgeColorToChart.GetValueOrDefault(status.BadgeColor, "#3b82f6"),
                    Count = myTasks.Count(t => t.TaskStatusId == status.Id)
                });
            }

            response.FilterOptions.Projects = myTasks
                .GroupBy(t => new { t.Requirement.ProjectId, t.Requirement.Project.Description })
                .OrderBy(g => g.Key.Description)
                .Select(g => new DashboardFilterOption { Id = g.Key.ProjectId, Label = g.Key.Description })
                .ToList();

            response.FilterOptions.TaskStatuses = taskStatuses
                .Select(s => new DashboardFilterOption { Id = s.Id, Label = s.Description })
                .ToList();

            response.FilterOptions.BugStatuses = bugStatuses
                .Select(s => new DashboardFilterOption { Id = s.Id, Label = s.Description })
                .ToList();

            IEnumerable<Data.Entities.TaskManagement.Task> filteredMyTasks = myTasks;
            if (request.ProjectId > 0)
                filteredMyTasks = filteredMyTasks.Where(t => t.Requirement.ProjectId == request.ProjectId);
            if (request.TaskStatusId > 0)
                filteredMyTasks = filteredMyTasks.Where(t => t.TaskStatusId == request.TaskStatusId);

            var filteredTaskList = filteredMyTasks.ToList();

            response.MyAssignedTasks = filteredTaskList
                .Where(t => !finalizedStatusIds.Contains(t.TaskStatusId))
                .OrderBy(t => t.TaskStatus.Order)
                .ThenByDescending(t => t.Id)
                .Select(t => MapMyTask(t, actualHoursByTask, bugCountByTask, timeLogsByTask))
                .ToList();

            response.MyDelayedTasks = myTasks
                .Where(t => !finalizedStatusIds.Contains(t.TaskStatusId))
                .Where(t => request.ProjectId <= 0 || t.Requirement.ProjectId == request.ProjectId)
                .Where(t => t.TimeEstimationHours > 0)
                .Select(t => MapMyTask(t, actualHoursByTask, bugCountByTask, timeLogsByTask))
                .Where(t => t.OvertimeHours > 0)
                .OrderByDescending(t => t.OvertimeHours)
                .ToList();

            response.OvertimeTasks = response.MyDelayedTasks
                .Take(20)
                .Select(t => new DashboardOvertimeAlert
                {
                    TaskId = t.TaskId,
                    TaskDescription = t.TaskDescription,
                    ProjectName = t.ProjectName,
                    ResponsibleName = string.Empty,
                    StatusName = t.StatusName,
                    PlannedHours = t.PlannedHours,
                    ActualHours = t.ActualHours,
                    OvertimeHours = t.OvertimeHours
                })
                .ToList();

            response.MyWorkedTasks = myTasks
                .Where(t => IsTaskActiveInRange(t, rangeStart, rangeEnd, timeLogsByTask))
                .OrderByDescending(t => actualHoursByTask.GetValueOrDefault(t.Id))
                .Select(t => MapMyTask(t, actualHoursByTask, bugCountByTask, timeLogsByTask))
                .ToList();

            var scopedTasks = myTasks
                .Where(t => request.ProjectId <= 0 || t.Requirement.ProjectId == request.ProjectId)
                .ToList();

            response.AssignedProjectsCount = scopedTasks
                .Select(t => t.Requirement.ProjectId)
                .Distinct()
                .Count();

            response.WorkedProjects = scopedTasks
                .Select(t => new
                {
                    t.Requirement.ProjectId,
                    ProjectName = t.Requirement.Project.Description,
                    HoursInPeriod = timeLogsByTask.GetValueOrDefault(t.Id)?
                        .Where(l => l.ExecutionDate.Date >= rangeStart.Date && l.ExecutionDate.Date <= rangeEnd.Date)
                        .Sum(l => l.UsedHours) ?? 0m
                })
                .Where(x => x.HoursInPeriod > 0)
                .GroupBy(x => new { x.ProjectId, x.ProjectName })
                .Select(g => new DashboardWorkedProjectSummary
                {
                    ProjectId = g.Key.ProjectId,
                    ProjectName = g.Key.ProjectName,
                    HoursInPeriod = g.Sum(x => x.HoursInPeriod)
                })
                .OrderByDescending(x => x.HoursInPeriod)
                .ToList();

            response.WorkedProjectsCount = response.WorkedProjects.Count;

            response.TopDeviatedTasks = response.MyWorkedTasks
                .Where(t => t.ActualHours > 0 && t.PlannedHours > 0)
                .OrderByDescending(t => Math.Abs(t.ActualHours - t.PlannedHours))
                .Take(5)
                .Select(t => new DashboardDeviatedTask
                {
                    TaskId = t.TaskId,
                    TaskDescription = t.TaskDescription,
                    ProjectName = t.ProjectName,
                    ResponsibleName = string.Empty,
                    PlannedHours = t.PlannedHours,
                    ActualHours = t.ActualHours,
                    DeviationPercent = t.DeviationPercent,
                    AbsoluteDeviationHours = Math.Abs(t.ActualHours - t.PlannedHours)
                })
                .ToList();

            var myBugsQuery = dbContext.TaskBug
                .AsNoTracking()
                .Include(b => b.Requirement)
                    .ThenInclude(r => r.Project)
                .Include(b => b.Task)
                .Include(b => b.TaskBugStatus)
                .Where(b => b.RowStatus == (short)RowStatus.Active
                    && b.TaskId != null
                    && b.Task!.UserId == userId);

            if (request.ProjectId > 0)
                myBugsQuery = myBugsQuery.Where(b => b.Requirement.ProjectId == request.ProjectId);
            if (request.BugStatusId > 0)
                myBugsQuery = myBugsQuery.Where(b => b.TaskBugStatusId == request.BugStatusId);

            response.MyAssignedBugs = myBugsQuery
                .OrderByDescending(b => b.Created)
                .Select(b => new DashboardMyBugItem
                {
                    BugId = b.Id,
                    Description = b.Description,
                    ProjectId = b.Requirement.ProjectId,
                    ProjectName = b.Requirement.Project.Description,
                    TaskId = b.TaskId,
                    TaskDescription = b.Task!.Description,
                    TaskBugStatusId = b.TaskBugStatusId,
                    StatusName = b.TaskBugStatus.Description,
                    StatusBadgeColor = b.TaskBugStatus.BadgeColor,
                    ReportedAt = b.Created
                })
                .ToList();

            var nextWeekEnd = today.AddDays(7);
            var teamTimeOffs = dbContext.UserTimeOff
                .AsNoTracking()
                .Include(t => t.User)
                .Where(t => t.RowStatus == (short)RowStatus.Active)
                .ToList();

            response.OutToday = teamTimeOffs
                .Where(t => t.StartDate.Date <= today && t.EndDate.Date >= today)
                .Select(MapTimeOffBanner)
                .OrderBy(t => t.UserName)
                .ToList();

            response.UpcomingVacations = teamTimeOffs
                .Where(t => t.Type == (short)UserTimeOffType.Vacation)
                .Where(t => t.StartDate.Date > today && t.StartDate.Date <= nextWeekEnd)
                .Select(MapTimeOffBanner)
                .OrderBy(t => t.StartDate)
                .ThenBy(t => t.UserName)
                .ToList();

            return OperationResult<DashboardReadResponse>.CreateSuccessResult(response);
        }
        catch (Exception ex)
        {
            return OperationResult<DashboardReadResponse>.CreateFailureResult(ex);
        }
    }

    private static DashboardMyTaskItem MapMyTask(
        Data.Entities.TaskManagement.Task task,
        Dictionary<long, decimal> actualHoursByTask,
        Dictionary<long, int> bugCountByTask,
        Dictionary<long, List<TimeLog>> timeLogsByTask)
    {
        var actual = actualHoursByTask.GetValueOrDefault(task.Id);
        var planned = task.TimeEstimationHours;
        var deviationPercent = planned > 0
            ? (double)((actual - planned) / planned * 100m)
            : 0;

        DateTime? lastWorkDate = null;
        if (timeLogsByTask.TryGetValue(task.Id, out var logs) && logs.Count > 0)
            lastWorkDate = logs.Max(x => x.ExecutionDate);

        return new DashboardMyTaskItem
        {
            TaskId = task.Id,
            TaskDescription = task.Description,
            ProjectId = task.Requirement.ProjectId,
            ProjectName = task.Requirement.Project.Description,
            TaskStatusId = task.TaskStatusId,
            StatusName = task.TaskStatus.Description,
            StatusBadgeColor = task.TaskStatus.BadgeColor,
            PlannedHours = planned,
            ActualHours = actual,
            BugCount = bugCountByTask.GetValueOrDefault(task.Id),
            OvertimeHours = Math.Max(0, actual - planned),
            DeviationPercent = Math.Round(deviationPercent, 1),
            LastWorkDate = lastWorkDate
        };
    }

    private static DashboardTimeOffBannerItem MapTimeOffBanner(Data.Entities.Security.UserTimeOff timeOff) =>
        new()
        {
            UserId = timeOff.UserId,
            UserName = $"{timeOff.User.Name} {timeOff.User.LastName}".Trim(),
            Type = timeOff.Type,
            TypeLabel = timeOff.Type == (short)UserTimeOffType.Vacation ? "Vacaciones" : "Permiso",
            StartDate = timeOff.StartDate,
            EndDate = timeOff.EndDate
        };

    private static bool IsTaskActiveInRange(
        Data.Entities.TaskManagement.Task task,
        DateTime rangeStart,
        DateTime rangeEnd,
        Dictionary<long, List<TimeLog>> timeLogsByTask)
    {
        if (timeLogsByTask.TryGetValue(task.Id, out var logs))
        {
            if (logs.Any(l => l.ExecutionDate.Date >= rangeStart.Date && l.ExecutionDate.Date <= rangeEnd.Date))
                return true;
        }

        if (task.ActualEndDate.HasValue
            && task.ActualEndDate.Value.Date >= rangeStart.Date
            && task.ActualEndDate.Value.Date <= rangeEnd.Date)
            return true;

        if (task.EndDate.HasValue
            && task.EndDate.Value.Date >= rangeStart.Date
            && task.EndDate.Value.Date <= rangeEnd.Date)
            return true;

        var taskEnd = task.ActualEndDate?.Date ?? task.EndDate?.Date ?? rangeEnd.Date;
        return task.StartDate.Date <= rangeEnd.Date && taskEnd >= rangeStart.Date;
    }
}
