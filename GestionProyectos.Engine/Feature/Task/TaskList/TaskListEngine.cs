using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Task;
using GestionProyectos.Engine.Feature.Task.TaskList.Request;
using GestionProyectos.Engine.Feature.Task.TaskList.Response;
using GestionProyectos.Engine.Security.Utilities;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Engine.Feature.Task.TaskList
{
    public class TaskListEngine : ITaskListEngine
    {
        private readonly DataDbContext dbContext;

        public TaskListEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public OperationResult<TaskListResponse> Execute(TaskListRequest request)
        {
            try
            {
                TaskListResponse response = new();

                var startDate = new DateTime(request.StartDate.Year, request.StartDate.Month, request.StartDate.Day, 0, 0, 0);
                var endDate = new DateTime(request.EndDate.Year, request.EndDate.Month, request.EndDate.Day, 23, 59, 59);

                var tasks = dbContext.Task
                    .Include(t => t.Requirement).ThenInclude(r => r.Project).ThenInclude(p => p.Customer)
                    .Include(t => t.User)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.DevelopmentPhase)
                    .Include(t => t.TimeLogs)
                    .Where(t => t.RowStatus == (short)RowStatus.Active)
                    .ToList();

                if (request.OnlyAssignedToMe)
                    tasks = tasks.Where(t => t.UserId == request.Context.UserId).ToList();
                else if (request.DeveloperId > 0)
                    tasks = tasks.Where(t => t.UserId == request.DeveloperId).ToList();

                if (request.EnterpriseId > 0)
                    tasks = tasks.Where(t => t.Requirement?.Project?.Customer?.EnterpriseId == request.EnterpriseId).ToList();

                if (request.CustomerId > 0)
                    tasks = tasks.Where(t => t.Requirement?.Project?.CustomerId == request.CustomerId).ToList();

                if (request.ProjectId > 0)
                    tasks = tasks.Where(t => t.Requirement?.ProjectId == request.ProjectId).ToList();

                if (request.RequirementId > 0)
                    tasks = tasks.Where(t => t.RequirementId == request.RequirementId).ToList();

                if (request.TaskStatusId > 0)
                    tasks = tasks.Where(t => t.TaskStatusId == request.TaskStatusId).ToList();

                var taskIds = tasks.Select(t => t.Id).ToList();
                var totalHoursByTask = dbContext.TimeLog
                    .Where(tl => taskIds.Contains(tl.TaskId) && tl.RowStatus == (short)RowStatus.Active)
                    .AsEnumerable()
                    .GroupBy(tl => tl.TaskId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.UsedHours));

                var bugCountByTask = dbContext.TaskBug
                    .Where(b => b.TaskId.HasValue && taskIds.Contains(b.TaskId.Value) && b.RowStatus == (short)RowStatus.Active)
                    .AsEnumerable()
                    .GroupBy(b => b.TaskId!.Value)
                    .ToDictionary(g => g.Key, g => g.Count());

                if (request.OnlyDelayed)
                {
                    tasks = tasks
                        .Where(t => totalHoursByTask.GetValueOrDefault(t.Id) > t.TimeEstimationHours)
                        .ToList();
                }

                if (request.OnlyWithBugs)
                {
                    tasks = tasks
                        .Where(t => bugCountByTask.GetValueOrDefault(t.Id) > 0)
                        .ToList();
                }

                var progressByTask = dbContext.TimeLog
                    .Where(tl => taskIds.Contains(tl.TaskId) && tl.RowStatus == (short)RowStatus.Active)
                    .AsEnumerable()
                    .GroupBy(tl => tl.TaskId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.ProgressPercent));

                foreach (var task in tasks.OrderByDescending(t => t.Id))
                {
                    var hoursInPeriod = task.TimeLogs
                        .Where(x => x.RowStatus == (short)RowStatus.Active
                                 && x.ExecutionDate >= startDate
                                 && x.ExecutionDate <= endDate)
                        .Sum(x => x.UsedHours);

                    var totalHoursWorked = totalHoursByTask.GetValueOrDefault(task.Id);
                    var bugCount = bugCountByTask.GetValueOrDefault(task.Id);
                    var progressPercent = (int)Math.Min(100, Math.Round(progressByTask.GetValueOrDefault(task.Id)));

                    var lastExecutionDate = task.TimeLogs
                        .Where(x => x.RowStatus == (short)RowStatus.Active)
                        .Select(x => x.ExecutionDate)
                        .DefaultIfEmpty()
                        .Max();

                    response.Tasks.Add(new TaskItem
                    {
                        Id = task.Id,
                        TicketId = task.RequirementId,
                        ProjectId = task.Requirement?.ProjectId ?? 0,
                        ProjectDescription = task.Requirement?.Project?.Description ?? string.Empty,
                        TaskDescription = task.Description,
                        TicketDescription = task.Requirement?.Description ?? string.Empty,
                        TimeEstimationHours = task.TimeEstimationHours,
                        TaskStatusId = task.TaskStatusId,
                        TaskStatus = task.TaskStatus?.Description ?? string.Empty,
                        UserId = task.UserId ?? 0,
                        UserName = $"{task.User?.Name} {task.User?.LastName}".Trim(),
                        HoursWorkedInPeriod = hoursInPeriod,
                        TotalHoursWorked = totalHoursWorked,
                        BugCount = bugCount,
                        ProgressPercent = progressPercent,
                        LastExecutionDate = lastExecutionDate == default ? null : lastExecutionDate,
                        IsWithinOriginalScope = task.IsWithinOriginalScope,
                        ScopeChangeReason = task.ScopeChangeReason,
                        DevelopmentPhaseId = task.DevelopmentPhaseId,
                        DevelopmentPhase = task.DevelopmentPhase?.Description ?? string.Empty
                    });
                }

                return OperationResult<TaskListResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<TaskListResponse>.CreateFailureResult(ex);
            }
        }
    }
}
