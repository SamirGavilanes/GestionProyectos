using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Performance.Request;
using GestionProyectos.Engine.Feature.Performance.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.Performance;

public class PerformanceEngine : IPerformanceEngine
{
    private static readonly string[] UserColors =
    {
        "#2563EB", "#EA580C", "#16A34A", "#DB2777", "#CA8A04",
        "#0891B2", "#7C3AED", "#DC2626", "#0D9488", "#4F46E5"
    };

    private readonly DataDbContext dbContext;

    public PerformanceEngine(DataDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public OperationResult<PerformanceReadResponse> Execute(PerformanceReadRequest request)
    {
        try
        {
            var startDate = new DateTime(request.StartDate.Year, request.StartDate.Month, request.StartDate.Day, 0, 0, 0);
            var endDate = new DateTime(request.EndDate.Year, request.EndDate.Month, request.EndDate.Day, 23, 59, 59);

            var tasksQuery = dbContext.Task
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.Requirement)
                    .ThenInclude(r => r.Project)
                        .ThenInclude(p => p.Customer)
                .Where(t => t.RowStatus == (short)RowStatus.Active && t.TimeEstimationHours > 0);

            if (IsDeveloperRole(request.Context))
                tasksQuery = tasksQuery.Where(t => t.UserId == request.Context.UserId);
            else if (request.DeveloperId > 0)
                tasksQuery = tasksQuery.Where(t => t.UserId == request.DeveloperId);

            if (request.EnterpriseId > 0)
                tasksQuery = tasksQuery.Where(t => t.Requirement.Project.Customer.EnterpriseId == request.EnterpriseId);

            if (request.CustomerId > 0)
                tasksQuery = tasksQuery.Where(t => t.Requirement.Project.CustomerId == request.CustomerId);

            if (request.ProjectId > 0)
                tasksQuery = tasksQuery.Where(t => t.Requirement.ProjectId == request.ProjectId);

            if (request.RequirementId > 0)
                tasksQuery = tasksQuery.Where(t => t.RequirementId == request.RequirementId);

            var tasks = tasksQuery.ToList();
            var taskIds = tasks.Select(t => t.Id).ToList();

            var totalHoursByTask = dbContext.TimeLog
                .AsNoTracking()
                .Where(tl => taskIds.Contains(tl.TaskId) && tl.RowStatus == (short)RowStatus.Active)
                .AsEnumerable()
                .GroupBy(tl => tl.TaskId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.UsedHours));

            Dictionary<long, decimal> hoursInPeriodByTask;
            if (request.UseDateRange)
            {
                hoursInPeriodByTask = dbContext.TimeLog
                    .AsNoTracking()
                    .Where(tl => taskIds.Contains(tl.TaskId) && tl.RowStatus == (short)RowStatus.Active)
                    .AsEnumerable()
                    .Where(tl => tl.ExecutionDate.Date >= startDate.Date && tl.ExecutionDate.Date <= endDate.Date)
                    .GroupBy(tl => tl.TaskId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.UsedHours));
            }
            else
            {
                hoursInPeriodByTask = new Dictionary<long, decimal>();
            }

            var bugCountByTask = dbContext.TaskBug
                .AsNoTracking()
                .Where(b => b.TaskId.HasValue && taskIds.Contains(b.TaskId.Value) && b.RowStatus == (short)RowStatus.Active)
                .AsEnumerable()
                .GroupBy(b => b.TaskId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            tasks = tasks
                .Where(t => totalHoursByTask.GetValueOrDefault(t.Id) > 0)
                .Where(t => !request.UseDateRange || IsTaskInPeriod(t, startDate, endDate, totalHoursByTask, hoursInPeriodByTask))
                .ToList();

            var userIds = tasks.Where(t => t.UserId.HasValue).Select(t => t.UserId!.Value).Distinct().ToList();
            var timeOffsByUser = dbContext.UserTimeOff
                .AsNoTracking()
                .Where(t => userIds.Contains(t.UserId) && t.RowStatus == (short)RowStatus.Active)
                .AsEnumerable()
                .GroupBy(t => t.UserId)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());

            var analysisStart = request.UseDateRange ? startDate.Date : tasks.Count > 0
                ? tasks.Min(t => t.StartDate.Date)
                : DateTime.Today;
            var analysisEnd = request.UseDateRange ? endDate.Date : tasks.Count > 0
                ? tasks.Max(t => (t.ActualEndDate ?? t.EndDate ?? DateTime.Today).Date)
                : DateTime.Today;

            var userColorMap = tasks
                .Select(t => new { UserId = t.UserId ?? 0, Name = $"{t.User?.Name} {t.User?.LastName}".Trim() })
                .DistinctBy(x => x.UserId)
                .OrderBy(x => x.Name)
                .Select((x, index) => new { x.UserId, Color = UserColors[index % UserColors.Length] })
                .ToDictionary(x => x.UserId, x => x.Color);

            var response = new PerformanceReadResponse();

            foreach (var task in tasks)
            {
                var actualHours = totalHoursByTask.GetValueOrDefault(task.Id);
                var plannedHours = task.TimeEstimationHours;
                var userId = task.UserId ?? 0;

                timeOffsByUser.TryGetValue(userId, out var userTimeOffs);
                var (windowStart, windowEnd) = PerformanceAbsenceHelper.GetTaskWindow(
                    task, startDate, endDate, request.UseDateRange);
                var calendarDays = Math.Max(1, (windowEnd - windowStart).Days + 1);
                var absentDays = PerformanceAbsenceHelper.CountAbsentDays(userTimeOffs ?? Enumerable.Empty<Data.Entities.Security.UserTimeOff>(), windowStart, windowEnd);
                var availability = PerformanceAbsenceHelper.GetAvailabilityRatio(calendarDays, absentDays);
                var normalizedActual = availability > 0 ? actualHours / availability : actualHours;

                var deviationPercent = plannedHours > 0
                    ? (double)((normalizedActual - plannedHours) / plannedHours * 100m)
                    : 0;

                response.Tasks.Add(new PerformanceTaskPoint
                {
                    TaskId = task.Id,
                    TaskDescription = task.Description,
                    UserId = userId,
                    UserName = $"{task.User?.Name} {task.User?.LastName}".Trim(),
                    ProjectName = task.Requirement.Project.Description,
                    PlannedHours = plannedHours,
                    ActualHours = actualHours,
                    DeviationPercent = Math.Round(deviationPercent, 1),
                    BugCount = bugCountByTask.GetValueOrDefault(task.Id),
                    Color = userColorMap.GetValueOrDefault(userId, UserColors[0])
                });
            }

            var byUser = response.Tasks.GroupBy(t => t.UserId);
            foreach (var group in byUser.OrderBy(g => g.First().UserName))
            {
                var userId = group.Key;
                timeOffsByUser.TryGetValue(userId, out var userTimeOffs);
                var absentDaysInPeriod = PerformanceAbsenceHelper.CountAbsentDays(
                    userTimeOffs ?? Enumerable.Empty<Data.Entities.Security.UserTimeOff>(),
                    analysisStart,
                    analysisEnd);

                var loggedHours = request.UseDateRange
                    ? group.Sum(t => hoursInPeriodByTask.GetValueOrDefault(t.TaskId))
                    : group.Sum(t => t.ActualHours);

                var mae = group.Average(t =>
                {
                    timeOffsByUser.TryGetValue(t.UserId, out var offs);
                    var taskEntity = tasks.First(x => x.Id == t.TaskId);
                    var (wStart, wEnd) = PerformanceAbsenceHelper.GetTaskWindow(taskEntity, startDate, endDate, request.UseDateRange);
                    var days = Math.Max(1, (wEnd - wStart).Days + 1);
                    var absent = PerformanceAbsenceHelper.CountAbsentDays(offs ?? Enumerable.Empty<Data.Entities.Security.UserTimeOff>(), wStart, wEnd);
                    var avail = PerformanceAbsenceHelper.GetAvailabilityRatio(days, absent);
                    var normalized = avail > 0 ? t.ActualHours / avail : t.ActualHours;
                    return Math.Abs((double)(normalized - t.PlannedHours));
                });

                response.Employees.Add(new PerformanceEmployeeSummary
                {
                    UserId = userId,
                    UserName = group.First().UserName,
                    MeanAbsoluteErrorHours = Math.Round(mae, 2),
                    TaskCount = group.Count(),
                    AbsentDays = absentDaysInPeriod,
                    LoggedHours = Math.Round(loggedHours, 2),
                    Color = userColorMap.GetValueOrDefault(userId, UserColors[0])
                });
            }

            return OperationResult<PerformanceReadResponse>.CreateSuccessResult(response);
        }
        catch (Exception ex)
        {
            return OperationResult<PerformanceReadResponse>.CreateFailureResult(ex);
        }
    }

    private static bool IsTaskInPeriod(
        Data.Entities.TaskManagement.Task task,
        DateTime startDate,
        DateTime endDate,
        Dictionary<long, decimal> totalHoursByTask,
        Dictionary<long, decimal> hoursInPeriodByTask)
    {
        if (hoursInPeriodByTask.GetValueOrDefault(task.Id) > 0)
            return true;

        if (task.ActualEndDate.HasValue
            && task.ActualEndDate.Value.Date >= startDate.Date
            && task.ActualEndDate.Value.Date <= endDate.Date)
            return true;

        if (task.EndDate.HasValue
            && task.EndDate.Value.Date >= startDate.Date
            && task.EndDate.Value.Date <= endDate.Date)
            return true;

        if (totalHoursByTask.GetValueOrDefault(task.Id) <= 0)
            return false;

        var periodStart = startDate.Date;
        var periodEnd = endDate.Date;
        var taskStart = task.StartDate.Date;
        var taskEnd = task.ActualEndDate?.Date ?? task.EndDate?.Date ?? periodEnd;

        return taskStart <= periodEnd && taskEnd >= periodStart;
    }

    private static bool IsDeveloperRole(Context context) =>
        context.Role?.Equals("Desarrollador", StringComparison.OrdinalIgnoreCase) == true;
}
