using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Project.Detail.Request;
using GestionProyectos.Engine.Feature.Project.Detail.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.Project.Detail;

public class ProjectDetailReadEngine : IProjectDetailReadEngine
{
    private readonly DataDbContext dbContext;

    public ProjectDetailReadEngine(DataDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public OperationResult<ProjectDetailReadResponse> Execute(ProjectDetailReadRequest request)
    {
        try
        {
            var project = dbContext.Project.FirstOrDefault(p =>
                p.Id == request.ProjectId && p.RowStatus == (short)RowStatus.Active);

            if (project == null)
                return OperationResult<ProjectDetailReadResponse>.CreateFailureResult("El proyecto no existe.");

            var startDate = new DateTime(request.StartDate.Year, request.StartDate.Month, request.StartDate.Day, 0, 0, 0);
            var endDate = new DateTime(request.EndDate.Year, request.EndDate.Month, request.EndDate.Day, 23, 59, 59);

            var requirements = dbContext.Requirement
                .Where(r => r.ProjectId == request.ProjectId && r.RowStatus == (short)RowStatus.Active)
                .OrderBy(r => r.Description)
                .ToList();

            if (request.RequirementStatusId > 0)
                requirements = requirements.Where(r => r.RequirementStatusId == request.RequirementStatusId).ToList();

            var requirementIds = requirements.Select(r => r.Id).ToList();

            var tasks = dbContext.Task
                .Include(t => t.TimeLogs)
                .Include(t => t.User)
                .Include(t => t.TaskStatus)
                .Where(t => requirementIds.Contains(t.RequirementId) && t.RowStatus == (short)RowStatus.Active)
                .ToList();

            if (IsDeveloperRole(request.Context))
                tasks = tasks.Where(t => t.UserId == request.Context.UserId).ToList();
            else if (request.ResponsibleUserId > 0)
                tasks = tasks.Where(t => t.UserId == request.ResponsibleUserId).ToList();

            if (request.TaskStatusId > 0)
                tasks = tasks.Where(t => t.TaskStatusId == request.TaskStatusId).ToList();

            var response = new ProjectDetailReadResponse
            {
                Project = new ProjectDetailInfo
                {
                    Id = project.Id,
                    Description = project.Description,
                    Customer = project.Customer.Description,
                    Status = project.ProjectStatus.Description,
                    IsClosedStatus = project.ProjectStatus.IsClosed,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    ActualEndDate = project.ActualEndDate
                }
            };

            foreach (var requirement in requirements)
            {
                var requirementTasks = tasks
                    .Where(t => t.RequirementId == requirement.Id)
                    .OrderBy(t => t.Description)
                    .ToList();

                if (requirementTasks.Count == 0)
                {
                    if (IsDeveloperRole(request.Context) || request.TaskStatusId > 0 || request.ResponsibleUserId > 0)
                        continue;
                }

                var requirementItem = new ProjectDetailRequirementItem
                {
                    Id = requirement.Id,
                    Description = requirement.Description,
                    RequirementStatusId = requirement.RequirementStatusId,
                    Status = requirement.RequirementStatus.Description
                };

                foreach (var task in requirementTasks)
                {
                    var activeLogs = task.TimeLogs
                        .Where(x => x.RowStatus == (short)RowStatus.Active)
                        .ToList();

                    var logsInPeriod = activeLogs
                        .Where(x => x.ExecutionDate >= startDate && x.ExecutionDate <= endDate)
                        .ToList();

                    var logsUpToPeriodEnd = activeLogs
                        .Where(x => x.ExecutionDate <= endDate)
                        .ToList();

                    var loggedHours = logsInPeriod.Sum(x => x.UsedHours);
                    var totalLoggedHours = logsUpToPeriodEnd.Sum(x => x.UsedHours);
                    DateTime? lastHoursUpdate = logsInPeriod.Count > 0
                        ? logsInPeriod.Max(x => x.Updated ?? x.Created)
                        : null;

                    var totalLoggedProgress = logsUpToPeriodEnd.Sum(x => x.ProgressPercent);

                    var lastExecutionDate = activeLogs.Count > 0
                        ? activeLogs.Max(x => x.ExecutionDate)
                        : (DateTime?)null;

                    var progressPercent = ResolveTaskProgressPercent(
                        task.TimeEstimationHours,
                        totalLoggedHours,
                        totalLoggedProgress > 0,
                        totalLoggedProgress);

                    requirementItem.Tasks.Add(new ProjectDetailTaskItem
                    {
                        Id = task.Id,
                        UserId = task.UserId ?? 0,
                        Description = task.Description,
                        Responsible = $"{task.User?.Name} {task.User?.LastName}".Trim(),
                        TaskStatusId = task.TaskStatusId,
                        Status = task.TaskStatus.Description,
                        EstimatedHours = task.TimeEstimationHours,
                        LoggedHours = loggedHours,
                        LastHoursUpdateAt = lastHoursUpdate,
                        LastExecutionDate = lastExecutionDate,
                        ProgressPercent = progressPercent
                    });
                }

                requirementItem.EstimatedHours = requirementItem.Tasks.Sum(t => t.EstimatedHours);
                requirementItem.LoggedHours = requirementItem.Tasks.Sum(t => t.LoggedHours);
                response.Requirements.Add(requirementItem);
            }

            response.TotalEstimatedHours = response.Requirements.Sum(r => r.EstimatedHours);
            response.TotalLoggedHours = response.Requirements.Sum(r => r.LoggedHours);
            response.Summary = BuildSummary(
                response.Requirements,
                response.TotalEstimatedHours,
                response.TotalLoggedHours,
                project.StartDate,
                project.EndDate,
                endDate);

            return OperationResult<ProjectDetailReadResponse>.CreateSuccessResult(response);
        }
        catch (Exception ex)
        {
            return OperationResult<ProjectDetailReadResponse>.CreateFailureResult(ex);
        }
    }

    private static ProjectDetailSummary BuildSummary(
        List<ProjectDetailRequirementItem> requirements,
        decimal totalEstimated,
        decimal totalLogged,
        DateTime projectStartDate,
        DateTime? projectEndDate,
        DateTime periodEndDate)
    {
        var allTasks = requirements.SelectMany(r => r.Tasks).ToList();
        var taskCount = allTasks.Count;
        var remaining = totalEstimated - totalLogged;

        var summary = new ProjectDetailSummary
        {
            EstimatedHours = totalEstimated,
            LoggedHours = totalLogged,
            RemainingHours = remaining,
            LoggedPercent = Percent(totalLogged, totalEstimated),
            RemainingPercent = Percent(Math.Abs(remaining), totalEstimated),
            TotalTasks = taskCount
        };

        decimal sumVariance = 0;
        var variancePercents = new List<decimal>();
        decimal weightedProgress = 0;
        decimal progressWeight = 0;
        var expectedProgress = GetExpectedProgressPercent(projectStartDate, projectEndDate, periodEndDate);
        const decimal scheduleTolerance = 2m;

        foreach (var task in allTasks)
        {
            var variance = task.LoggedHours - task.EstimatedHours;
            sumVariance += variance;

            if (variance < 0)
            {
                summary.HoursNegativeVariance += Math.Abs(variance);
                summary.TasksNegativeVariance++;
            }
            else if (variance > 0)
            {
                summary.HoursPositiveVariance += variance;
                summary.TasksPositiveVariance++;
            }
            else
            {
                summary.TasksOnTarget++;
            }

            if (task.EstimatedHours > 0)
                variancePercents.Add(variance / task.EstimatedHours * 100m);

            var weight = task.EstimatedHours > 0 ? task.EstimatedHours : 1m;
            weightedProgress += task.ProgressPercent * weight;
            progressWeight += weight;

            if (task.ProgressPercent > expectedProgress + scheduleTolerance)
                summary.TasksAheadSchedule++;
            else if (task.ProgressPercent < expectedProgress - scheduleTolerance)
                summary.TasksBehindSchedule++;
            else
                summary.TasksOnSchedule++;
        }

        summary.HoursNegativeVariancePercent = Percent(summary.HoursNegativeVariance, totalEstimated);
        summary.HoursPositiveVariancePercent = Percent(summary.HoursPositiveVariance, totalEstimated);
        summary.AverageHoursVariance = taskCount > 0 ? sumVariance / taskCount : 0;
        summary.TasksNegativeVariancePercent = Percent(summary.TasksNegativeVariance, taskCount);
        summary.TasksPositiveVariancePercent = Percent(summary.TasksPositiveVariance, taskCount);
        summary.AverageTaskVariancePercent = variancePercents.Count > 0
            ? variancePercents.Average()
            : 0;

        summary.ProgressPercent = progressWeight > 0
            ? Math.Round(weightedProgress / progressWeight, 1)
            : 0;
        summary.ExpectedProgressPercent = expectedProgress;
        summary.ScheduleVariancePercent = Math.Round(summary.ProgressPercent - expectedProgress, 1);
        summary.TasksAheadSchedulePercent = Percent(summary.TasksAheadSchedule, taskCount);
        summary.TasksBehindSchedulePercent = Percent(summary.TasksBehindSchedule, taskCount);

        summary.TaskStatusCounts = CountTaskStatuses(allTasks);

        return summary;
    }

    private static decimal ResolveTaskProgressPercent(
        decimal estimatedHours,
        decimal totalLoggedHours,
        bool hasLoggedProgress,
        decimal latestLogProgress)
    {
        if (hasLoggedProgress)
            return Math.Min(100, Math.Round(latestLogProgress, 1));

        if (estimatedHours > 0)
            return Math.Min(100, Math.Round(totalLoggedHours / estimatedHours * 100m, 1));

        return totalLoggedHours > 0 ? 100 : 0;
    }

    private static decimal GetExpectedProgressPercent(DateTime projectStartDate, DateTime? projectEndDate, DateTime asOfDate)
    {
        if (!projectEndDate.HasValue)
            return 0;

        var planStart = projectStartDate.Date;
        var planEnd = projectEndDate.Value.Date;
        var reference = asOfDate.Date;

        if (reference <= planStart)
            return 0;

        if (reference >= planEnd)
            return 100;

        var totalDays = (planEnd - planStart).Days;
        if (totalDays <= 0)
            return 100;

        return Math.Round((decimal)(reference - planStart).Days / totalDays * 100m, 1);
    }

    private static ProjectDetailTaskStatusCounts CountTaskStatuses(List<ProjectDetailTaskItem> tasks)
    {
        var counts = new ProjectDetailTaskStatusCounts();
        foreach (var task in tasks)
        {
            var status = task.Status.Trim();
            if (status.Equals("Pendiente", StringComparison.OrdinalIgnoreCase))
                counts.Pending++;
            else if (status.Contains("progreso", StringComparison.OrdinalIgnoreCase))
                counts.InProgress++;
            else if (status.Contains("finaliz", StringComparison.OrdinalIgnoreCase))
                counts.Finished++;
            else if (status.Contains("interno", StringComparison.OrdinalIgnoreCase))
                counts.InternalBlock++;
            else if (status.Contains("externo", StringComparison.OrdinalIgnoreCase))
                counts.ExternalBlock++;
        }

        return counts;
    }

    private static decimal Percent(decimal part, decimal total) =>
        total > 0 ? Math.Round(part / total * 100m, 1) : 0;

    private static bool IsDeveloperRole(Context context) =>
        context.Role?.Equals("Desarrollador", StringComparison.OrdinalIgnoreCase) == true;
}
